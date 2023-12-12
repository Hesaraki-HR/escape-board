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
    private Queue<Vector3> _wayPoints;


    CustomItemCollection<int> _diceResults;
    int _currentTileIndex;

    public CharacterReachedDestinationEvent onReachedDestination = new CharacterReachedDestinationEvent();

    public Vector3 DiceLocation => _diceLocation.position;
    public CinemachineVirtualCamera Camera => _virtualCamera;

    public Character()
    {
        _diceResults = new CustomItemCollection<int>();
        _wayPoints = new Queue<Vector3>();
        _currentTileIndex = 1;
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

    public void PlaceOn(Vector3 position)
    {
        transform.position = position;
        transform.gameObject.SetActive(true);
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

    public void CreateWayPoints()
    {
        _wayPoints.Clear();
        int steps = _diceResults.GetLastItem();
        var currentTile = Board.Instance.GetTile(_currentTileIndex);

        if (_currentTileIndex + steps <= Board.Instance.MaxIndex)
        {
            for (int i = 0; i < steps; i++)
            {
                var pos = _unitType == CharacterTypes.Player ? currentTile.NextTile.PlayerRoom : currentTile.NextTile.OpponentRoom;

                _wayPoints.Enqueue(new Vector3(pos.x, 0, pos.z));
                currentTile = currentTile.NextTile;
            }
        }
        else
        {
            onReachedDestination.Invoke(_currentTileIndex);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
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
        body.velocity = pushDir;
    }

}

