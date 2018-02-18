using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody2D;
    private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start ()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();

        inverseMoveTime = 1f / moveTime;
	}

    /// <summary>
    /// Checks to see if there is already something at the end position
    /// If there is not, it starts moving toward it.
    /// If there is it returns false
    /// </summary>
    /// <param name="xDir"></param>
    /// <param name="yDir"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Moves an object from its current position to an 'end' position
    /// </summary>
    /// <param name="end"></param>
    /// <returns></returns>
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDist = (transform.position - end).sqrMagnitude;

        while(sqrRemainingDist > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rigidBody2D.position, end, inverseMoveTime * Time.deltaTime);
            rigidBody2D.MovePosition(newPosition);

            sqrRemainingDist = (transform.position - end).sqrMagnitude;

            yield return null;
        }
    }

    /// <summary>
    /// Attempts to move an object to a target position
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xDir"></param>
    /// <param name="yDir"></param>
    protected virtual void AttemptMove<T> (int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if(hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if(!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
