using UnityEngine;
using System.Collections.Generic;

namespace AppCore
{
    public static class TransformExtensions 
    {
        // Returns base parent.
        public static Transform GetBaseParent(this Transform transform)
        {
            while (transform.parent != null)
                transform = transform.parent;

            return transform;
        }

        public static Transform[] ChildrenToArray(this Transform transform)
        {
            int childCount = transform.childCount;

            Transform[] children = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }

        private static void GetComponentsInChildrenRecursively<T>(this Transform transform, ref List<T> list) where T : Component 
        {
            int childCount = transform.childCount;

            for (int i = 0; i < childCount; i++) 
            {
                var child = transform.GetChild(i);
                var component = child.GetComponent<T>();

                child.GetComponentsInChildrenRecursively(ref list);

                if (component) 
                    list.Add(component);
            }
        }

        public static List<T> GetComponentsInChildrenRecursively<T>(this GameObject gameObject, bool includingParent = false) where T : Component
        {
            List<T> components = new List<T>();

            if (includingParent)
            {
                var component = gameObject.GetComponent<T>();

                if (component)
                    components.Add(component);
            }

            gameObject.transform.GetComponentsInChildrenRecursively(ref components);

            return components;
        }
    }

}