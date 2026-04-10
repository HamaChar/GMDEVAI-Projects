using UnityEngine;

public class Flee : NPCBaseFSM
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Calculate direction AWAY from the player
        var direction = NPC.transform.position - opponent.transform.position;
        
        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation,
            Quaternion.LookRotation(direction),
            rotSpeed * Time.deltaTime);
        
        // Move forward (which is now away from the player)
        NPC.transform.Translate(0, 0, Time.deltaTime * speed);
    }
}