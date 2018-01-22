using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eMapObjectType
{
    NONE,
    MONSTER,
}

public class MapObject : MonoBehaviour
{

    //Unity Functions

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}


    //Info

    protected eMapObjectType _type = eMapObjectType.NONE;

    public eMapObjectType GetObjectType()
    {
        return _type;
    }

    public void SetPosition(Vector2 position)
    {
        gameObject.transform.localPosition = position;
    }


    //Sorting

    virtual public void SetSortingOrder(eTileLayer layer, int sortingOrder)
    {
        _curLayer = layer;

        int sortingID = SortingLayer.NameToID(layer.ToString());
        gameObject.GetComponent<SpriteRenderer>().sortingLayerID = sortingID;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
    }


    //View

    public void BecomeViewer()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0.0f, 0.0f, Camera.main.transform.localPosition.z);
    }


    //Layer

    protected eTileLayer _curLayer;

    public eTileLayer GetCurrentLayer()
    {
        return _curLayer;
    }


    //Move

    protected bool _canMove = true;

    public bool CanMove()
    {
        return _canMove;
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }


    //Message

    virtual public void ReceiveObjectMessage(MessageParam msgParam)
    {
        switch(msgParam.message)
        {
            //case "Attack":
            //    //Debug.Log("Receive AttackMessage: " + msgParam.attackPoint);
            //    Damaged(msgParam.attackPoint);
            //    break;
        }
    }
}
