using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private const string IsWalking = "IsWalking";
    private Animator animator;
    [SerializeField] private Player player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
       
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(IsWalking, player.GetIsWalking());
    }
}
