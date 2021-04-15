using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AppCore
{
    public interface IProgressBarBehaviour
    {
        float Progress { get; set; }
        event Action<float> OnProgressChanged;
    }

    public class ImageProgressBarBehaviour : MonoBehaviour, IProgressBarBehaviour
    {
        [SerializeField] Image _image;

        public event Action<float> OnProgressChanged;

        private void Awake()
        {
            _image = _image ?? GetComponent<Image>();
        }

        public float Progress { get => _image.fillAmount; set { _image.fillAmount = value; OnProgressChanged?.Invoke(value); } }
        public Color Color { get { return _image.color; } set { _image.color = value; } }
        public Sprite Sprite { get { return _image.sprite; } set { _image.sprite = value; } }
    }
}