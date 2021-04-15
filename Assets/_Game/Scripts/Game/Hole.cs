using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private GameObject _goal;
    [SerializeField] private ParticleSystem _particle;
    public float speed;
    private int direction = 1;
    Vector3 _movement;
    public float hPosition;

    //public Tween tweenOne;
    //public Tween tweenSecond;

    void Update()
    {
        if(GameController.Instance.score >= 10)
        {
            if (transform.position.x > hPosition)
            {
                direction = -1;
            }
            else if (transform.position.x < -hPosition)
            {
                direction = 1;
            }
            _movement = Vector3.right * direction * speed * Time.deltaTime;
            transform.Translate(_movement);
        }
      
      
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            GameController.Instance.score += 1;
            EffectHole();
            _particle.Play();
            GameController.Instance.ChangeSpeedHole();
        }

    }

    void EffectHole()
    {
        _goal.GetComponent<SpriteRenderer>().sortingLayerName = "Other";
        _goal.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ResetHole()
    {
        _goal.GetComponent<SpriteRenderer>().sortingLayerName = "BackGround";
        _goal.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void ResetState()
    {
        DOTween.KillAll(true);
        gameObject.transform.position = new Vector3(0, transform.position.y, 0);
        speed = 0;
    }

    public void MoveHole()
    {


        //tweenOne =  gameObject.transform.DOLocalMoveX(2.5f, durationMoveHole).OnComplete(() =>
        //{
        //    tweenSecond = gameObject.transform.DOLocalMoveX(-2.5f, durationMoveHole).SetEase(Ease.Linear).OnUpdate(()=>
        //    { 
        //        if(Mathf.Abs(transform.position.x >= Mathf.Abs(hPosition.x))

        //            }).SetLoops(-1, LoopType.Yoyo);

        //}).SetEase(Ease.Linear);

        //Debug.Log(durationMoveHole + " duration");
    }
}

