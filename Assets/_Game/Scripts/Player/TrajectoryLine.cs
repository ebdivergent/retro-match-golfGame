using AppCore.Pooling;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
	public LineRenderer line;

	[SerializeField] GameObject _dotPrefab;
	public int _dotAmount;
	private float _dotGap;
	//private GameObject[] _dotArray;

	[SerializeField] SimplePool _pointsPool;
	[SerializeField] float _distanceBetweenPoints = 1f;

	void Start()
	{
		//Spawn Aim Line

		_dotGap = 1f / _dotAmount;
		SpawnPoint();

	}

	public void DrawLine(Vector3 startPoint, Vector3 endPoint)
	{
		line.positionCount = 2;
		Vector3[] allPoint = new Vector3[2];
		allPoint[0] = startPoint;
		allPoint[1] = endPoint;
		line.SetPositions(allPoint);
	}

	public void EndLine()
	{
		line.positionCount = 0;
	}

	void SpawnPoint(bool active = false)
	{
		//_dotArray = new GameObject[_dotAmount];

		//for (int i = 0; i < _dotAmount; i++)
		//{
		//	GameObject dot = Instantiate(_dotPrefab);
		//	dot.SetActive(active);
		//	_dotArray[i] = dot;
		//}
	}


	public void SetPositionsDots(Vector3 startMPos, Vector3 endMPos)
	{
		int pointsCount = (int)(Vector3.Distance(startMPos, endMPos) / _distanceBetweenPoints);

		Vector3 position = Vector3.zero;
		int checkValue = Mathf.Max(_pointsPool.ActiveItems.Count, pointsCount);
		for (int i = 0; i < checkValue; i++)
		{
			position = startMPos + (endMPos - startMPos).normalized * (_distanceBetweenPoints * (i + 1));
			if (i > _pointsPool.ActiveItems.Count - 1)
			{
				_pointsPool.Get(position);
			}
			else if (i < pointsCount)
			{
				_pointsPool.ActiveItems[i].transform.position = position;
			}
			else
			{
				_pointsPool.ReturnLastActive(_pointsPool.ActiveItems.Count - i);
				break;
			}
		}


		//for (int i = 0; i < _dotAmount; i++)
		//{
		//	Vector3 targetPos = Vector2.Lerp(startMPos, endMPos, i * _dotGap);
		//	_dotArray[i].transform.position = targetPos;
		//}
	}

	public void ChangeDotsActiveState(bool state)
	{
		_pointsPool.ReturnAll();
		//for (int i = 0; i < _dotAmount; i++)
		//{
		//	_dotArray[i].SetActive(state);
		//}
	}

}

