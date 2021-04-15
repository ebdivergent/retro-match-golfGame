using AppCore;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[HideInInspector] public Rigidbody2D rb;
	[HideInInspector] public CircleCollider2D col;


	[SerializeField] private GameObject _golfStick;

	[SerializeField] float _duration;
	[SerializeField] float _ballPower;

	[SerializeField] int _vibration;
	[SerializeField] int _elasticity;

	public TrajectoryLine trajectoryLine;
	public bool canPush;
	public int life;
	public int numOflife;
	
	private bool _isDrawLine;
	private Vector3 _startDragPos;
	private Vector3 _currentPos;
	private Vector2 _force;


	public Camera _camera;
	[HideInInspector] public Vector3 pos { get { return transform.position; } }

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		col = GetComponent<CircleCollider2D>();
		trajectoryLine = GetComponent<TrajectoryLine>();
	}

	void Start()
	{
		canPush = false;
		_camera = Camera.main;
		life = 3;
	}

	void Update()
	{
		if(GameController.Instance.CurrentGameState == GameState.GamePlay)
		{
			if (Input.GetMouseButtonDown(0))
			{
				canPush = true;
				_isDrawLine = true;
				rb.isKinematic = false;
				trajectoryLine.ChangeDotsActiveState(true);
				_startDragPos = GetWorldTouchPosition();
			}

		}
		if (canPush)
		{
			if (life > numOflife)
			{
				life = numOflife;
			}

			if (Input.GetMouseButtonUp(0) && canPush && GameController.Instance.CurrentGameState == GameState.GamePlay)
			{
				KickAnimation();
				_force = Vector3.ClampMagnitude(_currentPos - _startDragPos, 15);
				rb.AddForce(_force * _ballPower, ForceMode2D.Impulse);
				trajectoryLine.EndLine();
				trajectoryLine.ChangeDotsActiveState(false);

				_isDrawLine = false;

				GameController.Instance.SoundManager.EffectHit();
			}
		}
		
	}

	void LateUpdate()
	{
		if (canPush)
			if (_isDrawLine)
			{
				_currentPos = GetWorldTouchPosition();
				trajectoryLine.SetPositionsDots(transform.position, transform.position + (_currentPos - _startDragPos));
				//trajectoryLine.DrawLine(transform.position, transform.position + (_currentPos - _startDragPos));
			}
	}

	Vector3 GetWorldTouchPosition()
	{
		Plane plane = new Plane(Vector3.back, Vector3.zero);
		float point = 0;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (plane.Raycast(ray,out point))
		{
			return ray.GetPoint(point);
		}
		return Vector3.zero;
	}

	void KickAnimation()
	{
		//_golfStick.transform.DOPunchRotation(new Vector3(0, 0, -45), _duration, _vibration, _elasticity);
		_golfStick.transform.DORotate(new Vector3(0, 0, -45), 0.1f).OnComplete(() => { _golfStick.transform.DORotate(new Vector3(0, 0, 0), 0.1f); });
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "Goal")
		{
			gameObject.transform.DOScale(new Vector3(1.2f, 0, 0), 0.1f).OnComplete(() => Reset());
		}
		if (collider.gameObject.tag == "Wall")
		{
			life -= 1;
			GameController.Instance.SoundManager.EffectMissHealth();
			Reset();
		}
	}

	public void Reset()
	{
		GameController.Instance.Hole.ResetHole();

		gameObject.SetActive(true);
		gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 0);
		gameObject.transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f, _vibration, _elasticity);
		canPush = true;
		rb.velocity = Vector2.zero;
		rb.isKinematic = true;

		gameObject.transform.position = new Vector2(0, -3);
		gameObject.transform.Rotate(new Vector3(0, 0, 0));

	}
}
