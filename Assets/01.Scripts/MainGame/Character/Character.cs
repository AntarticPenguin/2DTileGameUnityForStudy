using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eMoveDirection
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN,
}

public class Character : MapObject
{
    protected GameObject _characterView;

    protected int _tileX = 0;
    protected int _tileY = 0;

    protected bool _isLive = true;
    protected int _hp = 100;

    protected int _attackPoint = 10;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateAttackCoolTime();
        _state.Update();

        //UI
        UpdateSlider();
    }

    //Init

    public void Init(string viewName)
    {
        //View를 붙인다(실제로 보일 모습(이미지), 카메라 아님)
        //Attach Player's View.
        string filePath = "Prefabs/CharacterView/" + viewName;
        GameObject characterViewPrefabs = Resources.Load<GameObject>(filePath);

        _characterView = GameObject.Instantiate(characterViewPrefabs);
        _characterView.transform.SetParent(transform);
        _characterView.transform.localPosition = Vector3.zero;
        _characterView.transform.localScale = Vector3.one;

        SetCanMove(false);

        TileMap map = GameManager.Instance.GetMap();

        _tileX = Random.Range(1, map.GetWidth() - 2);
        _tileY = Random.Range(1, map.GetHeight() - 2);
        map.SetObject(_tileX, _tileY, this, eTileLayer.MIDDLE);

        InitState();
    }


    //State

    protected Dictionary<eStateType, State> _stateMap = new Dictionary<eStateType, State>();
    protected State _state;

    virtual public void InitState()
    {
        {
            State state = new IdleState();
            state.Init(this);
            _stateMap[eStateType.IDLE] = state;
        }
        {
            State state = new MoveState();
            state.Init(this);
            _stateMap[eStateType.MOVE] = state;
        }
        {
            State state = new AttackState();
            state.Init(this);
            _stateMap[eStateType.ATTACK] = state;
        }
        {
            State state = new DamagedState();
            state.Init(this);
            _stateMap[eStateType.DAMAGED] = state;
        }
        {
            State state = new DeadState();
            state.Init(this);
            _stateMap[eStateType.DEAD] = state;
        }
        _state = _stateMap[eStateType.IDLE];
    }

    public void ChangeState(eStateType nextState)
    {
        if(null != _state)
            _state.Stop();
        _state = _stateMap[nextState];
        _state.Start();
    }

    override public void SetSortingOrder(eTileLayer layer, int sortingOrder)
    {
        _curLayer = layer;

        int sortingID = SortingLayer.NameToID(layer.ToString());
        _characterView.GetComponent<SpriteRenderer>().sortingLayerID = sortingID;
        _characterView.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
    }

    public int GetTileX() { return _tileX; }
    public int GetTileY() { return _tileY; }

    eMoveDirection _nextDirection = eMoveDirection.NONE;

    public eMoveDirection GetNextDirection() { return _nextDirection; }
    public void SetNextDirection(eMoveDirection direction) { _nextDirection = direction; }

    public bool MoveStart(int tileX, int tileY)
    {
        string animationTrigger = "up";

        switch (_nextDirection)
        {
            case eMoveDirection.LEFT: animationTrigger = "left";
                break;
            case eMoveDirection.RIGHT: animationTrigger = "right";
                break;
            case eMoveDirection.UP: animationTrigger = "up";
                break;
            case eMoveDirection.DOWN: animationTrigger = "down";
                break;
        }

        _characterView.GetComponent<Animator>().SetTrigger(animationTrigger);

        TileMap map = GameManager.Instance.GetMap();
        List<MapObject> collisionList = map.GetCollisionList(tileX, tileY);
        if (0 == collisionList.Count)
        {
            map.ResetObject(_tileX, _tileY, this);
            _tileX = tileX;
            _tileY = tileY;
            map.SetObject(_tileX, _tileY, this, eTileLayer.MIDDLE);

            return true;
        }
        return false;
    }


    //Message

    override public void ReceiveObjectMessage(MessageParam msgParam)
    {
        switch (msgParam.message)
        {
            case "Attack":
                _damagedPoint = msgParam.attackPoint;
                _state.NextState(eStateType.DAMAGED);
                break;
        }
    }


    //Attack

    float _attackCooltimeDuration = 0.0f;
    float _attackCooltime = 1.0f;

    public void Attack(MapObject enemy)
    {
        MessageParam msgParam = new MessageParam();
        msgParam.sender = this;
        msgParam.receiver = enemy;
        msgParam.message = "Attack";
        msgParam.attackPoint = _attackPoint;

        MessageSystem.Instance.Send(msgParam);
    }

    void UpdateAttackCoolTime()
    {
        if (_attackCooltimeDuration < _attackCooltime)
        {
            _attackCooltimeDuration += Time.deltaTime;
        }
        else
        {
            _attackCooltimeDuration = _attackCooltime;
        }
    }

    public bool IsAttackCoolDown()
    {
        if (_attackCooltime <= _attackCooltimeDuration)
            return true;
        return false;
    }

    public void ResetAttackCoolTime()
    {
        _attackCooltimeDuration = 0.0f;
    }

    int _damagedPoint = 0;

    public int GetDamagedPoint()
    {
        return _damagedPoint;
    }

    public void DecreaseHP(int damagedPoint)
    {
        _characterView.GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);

        _hp -= damagedPoint;
        if (_hp <= 0)
        {
            _hp = 0;
            _isLive = false;
        }
    }

    void ResetColor()
    {
        _characterView.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool IsLive()
    {
        return _isLive;
    }


    //UI

    Slider _hpGuage;
    Slider _cooltimeGuage;

    public void LinkHPGuage(Slider hpGuage)
    {
        GameObject canvasObject = transform.Find("Canvas").gameObject;
        hpGuage.transform.SetParent(canvasObject.transform);
        hpGuage.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        hpGuage.transform.localScale = Vector3.one;

        _hpGuage = hpGuage;
        _hpGuage.value = _hp / 100.0f;
    }

    public void LinkCooltimeGuage(Slider cooltimeGuage)
    {
        GameObject canvasObject = transform.Find("Canvas").gameObject;
        cooltimeGuage.transform.SetParent(canvasObject.transform);
        cooltimeGuage.transform.localPosition = Vector3.zero;
        cooltimeGuage.transform.localScale = Vector3.one;

        _cooltimeGuage = cooltimeGuage;
        _cooltimeGuage.value = _attackCooltimeDuration;
    }

    void UpdateSlider()
    {
        _hpGuage.value = _hp / 100.0f;
        _cooltimeGuage.value = _attackCooltimeDuration;
    }
}
