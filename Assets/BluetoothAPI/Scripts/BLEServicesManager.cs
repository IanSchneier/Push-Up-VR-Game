using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;

public class BLEServicesManager : MonoBehaviour
{
    private BluetoothHelper bluetoothHelper;
    void Start()
    {
        try{
            Debug.Log("HI");
            BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance();
            bluetoothHelper.OnConnected += () => {
                Debug.Log("Connected");
                sendData();
            };
            bluetoothHelper.OnConnectionFailed += ()=>{
                Debug.Log("Connection failed");
            };
            bluetoothHelper.OnScanEnded += OnScanEnded;
            bluetoothHelper.OnServiceNotFound += (serviceName) =>
            {
                Debug.Log(serviceName);
            };
            bluetoothHelper.OnCharacteristicNotFound += (serviceName, characteristicName) =>
            {
                Debug.Log(characteristicName);
            };
            bluetoothHelper.OnCharacteristicChanged += (value, characteristic) =>
            {
                Debug.Log(characteristic.getName());
                Debug.Log(System.Text.Encoding.ASCII.GetString(value));
            };

            BluetoothHelperService service = new BluetoothHelperService("FFE0");
            service.addCharacteristic(new BluetoothHelperCharacteristic("FFE1"));
            BluetoothHelperService service2 = new BluetoothHelperService("180A");
            service.addCharacteristic(new BluetoothHelperCharacteristic("2A24"));
            bluetoothHelper.Subscribe(service);
            bluetoothHelper.Subscribe(service2);
            bluetoothHelper.ScanNearbyDevices();

        }catch(Exception ex){
            Debug.Log(ex.Message);
        }
    }

    private void OnScanEnded(LinkedList<BluetoothDevice> devices){
        Debug.Log("FOund " + devices.Count);
        if(devices.Count == 0)
            return;
        try
        {
            bluetoothHelper.setDeviceName("HC-08");
            bluetoothHelper.Connect();
            Debug.Log("Connecting");
        }catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
       
    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }

    void sendData(){
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("FFE1");
        ch.setService("FFE0"); //this line is mandatory!!!
        bluetoothHelper.WriteCharacteristic(ch, "Hello");
    }

    void read(){
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("2A24");
        ch.setService("180A");//this line is mandatory!!!

        bluetoothHelper.ReadCharacteristic(ch);
        //Debug.Log(System.Text.Encoding.ASCII.GetString(x));
    }

}
