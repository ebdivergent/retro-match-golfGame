using AppCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppCore
{
    public class Page : UIElement
    {
        [SerializeField] PageType _page;
        [SerializeField] int _order;

        public int Order { get { return _order; } }
        public PageType PageType { get { return _page; } }
    }
}