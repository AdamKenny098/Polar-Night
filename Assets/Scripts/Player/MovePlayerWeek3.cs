using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerWeek3 : MonoBehaviour
{
    public static MovePlayerWeek3 Instance;
    [SerializeField] public float speed = 10;
    [SerializeField] public float sprintSpeed;
    [SerializeField] public float rotateSpeed = 3;
    [SerializeField] public float currentSpeed;
    CharacterController controller;
    public bool isSprinting = false;

    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        sprintSpeed = speed * 2;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = speed * Input.GetAxis("Vertical");
        }
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        controller.SimpleMove(forward * currentSpeed);


    }
}
