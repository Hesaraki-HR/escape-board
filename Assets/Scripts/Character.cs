using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterTypes
{
    Player,
    Opponent
}
public class CharacterReachedDestinationEvent : UnityEvent<int> { }

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    [SerializeField]
    float _moveSpeed = 1f;

    [SerializeField]
    float _rotationSpeed = 5f;

    [SerializeField]
    CharacterTypes _unitType = default;

    [SerializeField]
    CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private Transform _diceLocation;


    private CharacterController _characterController;
    private Animator _animator;
    protected Queue<Vector3> _wayPoints;


    CustomItemCollection<int> _diceResults;
    int _currentTileIndex;

    public CharacterReachedDestinationEvent onReachedDestination = new CharacterReachedDestinationEvent();

    public Vector3 DiceLocation => _diceLocation.position;
    public CinemachineVirtualCamera Camera => _virtualCamera;

    public Character()
    {
        _diceResults = new CustomItemCollection<int>();
        _wayPoints = new Queue<Vector3>();
        _currentTileIndex = 0;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    public void AddDiceResult(int diceResult)
    {
        _diceResults.AddItem(diceResult);
    }

    private void Update()
    {
        if (_wayPoints.Count > 0)
        {
            Vector3 nextWaypoint = _wayPoints.Peek();
            Vector3 direction = nextWaypoint - transform.position;

            _characterController.Move(direction.normalized * _moveSpeed * Time.deltaTime);

            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);


            _animator.SetTrigger("walk");

            Vector3 currentPosition = transform.position;
            currentPosition.y = nextWaypoint.y; // Match the Y position to the next waypoint
            float distanceToWaypoint = Vector3.Distance(currentPosition, nextWaypoint);
            if (distanceToWaypoint <= 0.1f)
            {
                _wayPoints.Dequeue();

                if (_wayPoints.Count == 0)
                {
                    _currentTileIndex += _diceResults.GetLastItem();
                    onReachedDestination.Invoke(_currentTileIndex);
                    _animator.ResetTrigger("walk");
                    _animator.SetTrigger("idle");
                }
            }
        }
    }

    // This script pushes all rigidbodies that the character touches
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float pushPower = 2.0F;
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }

    public void CreateWayPoints()
    {
        _wayPoints.Clear();
        int steps = _diceResults.GetLastItem();
        Tile currentTile;

        //Edge case for first move
        if (_currentTileIndex == 0)
        {
            var firstTile = Board.Instance.GetTile(1);
            var pos = _unitType == CharacterTypes.Player ? firstTile.PlayerRoom.position : firstTile.OpponentRoom.position;
            _wayPoints.Enqueue(new Vector3(pos.x, 0, pos.z));
            currentTile = firstTile;
            steps--;
        }
        else
        {
            currentTile = Board.Instance.GetTile(_currentTileIndex);
        }


        if (_currentTileIndex + steps <= Board.Instance.MaxIndex)
        {
            for (int i = 0; i < steps; i++)
            {
                var pos = _unitType == CharacterTypes.Player ? currentTile.NextTile.PlayerRoom.position : currentTile.NextTile.OpponentRoom.position;

                _wayPoints.Enqueue(new Vector3(pos.x, 0, pos.z));
                currentTile = currentTile.NextTile;
            }
        }
        else
        {
            onReachedDestination.Invoke(_currentTileIndex);
        }
    }

    public void TeleportTo(int targetIndex)
    {
        var targetTile = Board.Instance.GetTile(targetIndex);
        
        var pos = _unitType == CharacterTypes.Player ? targetTile.PlayerRoom.position : targetTile.OpponentRoom.position;
        _characterController.enabled = false;
        transform.position = pos;
        
        if(targetTile.NextTile != null) {
            var nextPos = _unitType == CharacterTypes.Player ? targetTile.NextTile.PlayerRoom.position : targetTile.NextTile.OpponentRoom.position;
            Vector3 direction = nextPos - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
        _currentTileIndex = targetIndex;

        _characterController.enabled = true;

        onReachedDestination.Invoke(_currentTileIndex);
    }

}

