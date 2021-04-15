using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
	[SerializeField] GameObject _object;

	List<GameObject> _activeItems = new List<GameObject>();
	List<GameObject> _disabledItems = new List<GameObject>();

	public List<GameObject> ActiveItems => _activeItems;

	public GameObject Get(Vector3 position)
	{
		if (_disabledItems.Count == 0)
		{
			_activeItems.Add(GameObject.Instantiate(_object, position, Quaternion.identity, transform));
		}
		else
		{
			_activeItems.Add(_disabledItems[0]);
			_disabledItems.RemoveAt(0);
			_activeItems[_activeItems.Count - 1].transform.position = position;
			_activeItems[_activeItems.Count - 1].SetActive(true);
		}

		return _activeItems[_activeItems.Count - 1];
	}

	public void Return(GameObject go)
	{
		if(_activeItems.Contains(go))
		{
			go.SetActive(false);
			_disabledItems.Add(go);
			_activeItems.Remove(go);
		}
	}

	public void ReturnLastActive(int count)
	{
		for(int i = 0; i < count; i++)
			Return(_activeItems[_activeItems.Count - 1]);
	}

	public void ReturnAll()
	{
		for (int i = 0; i < _activeItems.Count; i++)
		{
			_disabledItems.Add(_activeItems[i]);
			_activeItems[i].SetActive(false);
		}
		_activeItems.Clear();
	}
}
