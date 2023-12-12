using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class DiceRollResultEvent : UnityEvent<int> { }
public class Dice : MonoBehaviour
{
    [SerializeField] 
    int[] SidesValues = new int[] { 1, 2, 3, 4, 5, 6 };

    [SerializeField] 
    Transform[] Sides;

    [SerializeField] 
    float forceAmount = 2f;

    [SerializeField]
    CinemachineVirtualCamera _virtualCamera;


    Rigidbody _rb;

    private bool isRolling = false;

    public DiceRollResultEvent onDiceRollResult = new DiceRollResultEvent();

    public CinemachineVirtualCamera Camera => _virtualCamera;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Roll()
    {
        if (!isRolling)
        {
            _rb.isKinematic = false;
            Vector3 forceVecotr = Random.onUnitSphere.LimitToNorthernHemisphere();
            Vector3 torqueVector = Vector3.up.GetDeviatedDirectionFromUp(30f);

            _rb.AddForce(forceVecotr * forceAmount, ForceMode.Impulse);
            _rb.AddTorque(torqueVector * forceAmount, ForceMode.Impulse);
            isRolling = true;
        }
    }

    private void FixedUpdate()
    {
        if (isRolling && _rb.IsSleeping())
        {
            int result = DetermineDiceResult();
            onDiceRollResult.Invoke(result);
            isRolling = false;
        }
    }

    private int DetermineDiceResult()
    {
        int winnerSide = 0;
        for (int i = 0; i < Sides.Length; i++)
        {
            if (Sides[i].position.y > Sides[winnerSide].position.y)
            {
                winnerSide = i;
            }
        }
        return SidesValues[winnerSide];
    }

    public void PlaceOn(Vector3 position)
    {
        if(_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }

        _rb.isKinematic = true;
        transform.position = position + Vector3.up * 0.25f;
        Vector3 randomRotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        transform.rotation = Quaternion.Euler(randomRotation);
        transform.gameObject.SetActive(true);
    }
}
