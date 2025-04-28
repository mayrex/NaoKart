import joblib
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from tensorflow import keras
from tensorflow.keras import layers
from tensorflow.keras.callbacks import EarlyStopping, ModelCheckpoint

# === 1. Carica il dataset ===
data = pd.read_csv('Telemetry_Giro_01.csv')

# === 2. Prepara Input (X) e Output (y) ===
X = data[['Time', 'PosX', 'PosY', 'PosZ', 'VelX', 'VelY', 'VelZ', 
          'LocalVelX', 'LocalVelY', 'LocalVelZ', 'AccLat', 'AccLong', 'AccVert',
          'Yaw', 'Pitch', 'Roll', 'DistFromIdeal']].values

y = data[['Steering', 'Throttle', 'Brake']].values

# === 3. Normalizza input e output ===
scaler_x = MinMaxScaler()
scaler_y = MinMaxScaler()

X = scaler_x.fit_transform(X)
y = scaler_y.fit_transform(y)

joblib.dump(scaler_y, 'scaler_y.pkl')

# === 4. Split Train/Test ===
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# === 5. Crea il modello ===
model = keras.Sequential([
    layers.Dense(128, activation='relu', input_shape=(X.shape[1],)),
    layers.Dropout(0.2),  # aggiunto Dropout per ridurre overfitting
    layers.Dense(128, activation='relu'),
    layers.Dropout(0.2),
    layers.Dense(64, activation='relu'),
    layers.Dense(32, activation='relu'),
    layers.Dense(3)  # output: Steering, Throttle, Brake
])

# === 6. Compila ===
model.compile(optimizer=keras.optimizers.Adam(learning_rate=0.001),
              loss='mse',
              metrics=['mae'])

# === 7. EarlyStopping e ModelCheckpoint ===
early_stopping = EarlyStopping(
    monitor='val_loss',
    patience=10,   # fermati se per 10 epoche non migliora
    restore_best_weights=True
)

checkpoint = ModelCheckpoint(
    'best_model.h5',  # salva il modello migliore
    monitor='val_loss',
    save_best_only=True,
    mode='min'
)

# === 8. Allena ===
history = model.fit(
    X_train, y_train, 
    epochs=200,
    batch_size=32,
    validation_split=0.2,
    callbacks=[early_stopping, checkpoint]
)

# === 9. Valuta ===
test_loss, test_mae = model.evaluate(X_test, y_test)
print(f"Test Loss (MSE): {test_loss:.4f}, Test MAE: {test_mae:.4f}")

# === 10. Grafico delle perdite ===
plt.figure(figsize=(10,6))
plt.plot(history.history['loss'], label='Training Loss')
plt.plot(history.history['val_loss'], label='Validation Loss')
plt.xlabel('Epoche')
plt.ylabel('Loss (MSE)')
plt.legend()
plt.grid(True)
plt.title('Andamento della Loss durante l\'allenamento')
plt.show()

# === 11. Predizioni riscalate ===
y_pred = model.predict(X_test)
y_pred_rescaled = scaler_y.inverse_transform(y_pred)
y_test_rescaled = scaler_y.inverse_transform(y_test)
