using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

namespace AppCore
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        private bool _hasConnection;
        public bool HasConnection 
        { 
            get 
            { 
                return _hasConnection; 
            }
            private set
            {
                if (_hasConnection != value)
                {
                    _hasConnection = value;
                    OnChangeInternetAvailability?.Invoke(value);
                }
            } 
        }

        public event Action<bool> OnChangeInternetAvailability;

        private void Awake()
        {
            if (Instance != null)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(PingUpdate());
        }

        IEnumerator PingUpdate()
        {
            IPHostEntry host = null;
            try
            {
                host = Dns.GetHostEntry("google.com");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                HasConnection = false;
            }

            if (host != null && host.AddressList.Length > 0)
            {
                //Debug.Log("Adresses: \n" + string.Join(";\n", host.AddressList.Select(adr => adr.ToString())));
                Ping ping = new Ping(host.AddressList.First().ToString());
                yield return new WaitUntil(() => ping.isDone || ping.time >= 5000);

                if (ping.isDone == false)
                {
                    HasConnection = false;
                }
                else
                {
                    HasConnection = true;
                }
                ping.DestroyPing();
            }
            else
            {
                HasConnection = false;
            }

            yield return new WaitForSeconds(1f);
            StartCoroutine(PingUpdate());
        }
    }
}