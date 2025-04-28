import pandas as pd
import numpy as np
from sklearn.preprocessing import MinMaxScaler
from tensorflow.keras.models import load_model
import joblib

# Carica il modello e lo scaler salvato
model = load_model('model/best1.h5')
scaler_y = joblib.load('model/scaler_y.pkl')  # Carica lo scaler salvato

# === 1. Carica il CSV del giro sbagliato ===
data_sbagliata = pd.read_csv('Telemetry_Giro_005.csv')  # il tuo file csv

# === 2. Prepara gli input del giro sbagliato ===
X_sbagliato = data_sbagliata[['Time', 'PosX', 'PosY', 'PosZ', 'VelX', 'VelY', 'VelZ', 
                              'LocalVelX', 'LocalVelY', 'LocalVelZ', 'AccLat', 'AccLong', 'AccVert',
                              'Yaw', 'Pitch', 'Roll', 'DistFromIdeal']].values

# **Normalizza i dati** come prima
scaler_x = MinMaxScaler()
X_sbagliato_normalizzato = scaler_x.fit_transform(X_sbagliato)

# === 3. Fai delle previsioni sui dati sbagliati ===
y_pred = model.predict(X_sbagliato_normalizzato)

# === 4. Inverti la normalizzazione dei valori predetti ===
y_pred_rescaled = scaler_y.inverse_transform(y_pred)

# === 5. Confronta i risultati predetti con i valori reali ===
y_reali = data_sbagliata[['Steering', 'Throttle', 'Brake']].values
y_reali_rescaled = scaler_y.inverse_transform(y_reali)

# === 6. Calcola gli errori ===
errori = y_pred_rescaled - y_reali_rescaled
print("Differenza tra previsioni e valori reali (errori):")
print(errori)

# === 7. Calcola l'errore medio assoluto (MAE) ===
errore_mae = np.mean(np.abs(errori), axis=0)
print(f"Errore medio assoluto per Steering: {errore_mae[0]:.4f}, Throttle: {errore_mae[1]:.4f}, Brake: {errore_mae[2]:.4f}")
