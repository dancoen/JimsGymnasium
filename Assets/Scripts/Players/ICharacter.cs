using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{ 
    //This interface sets up movement distances and health.  Attacks will be another interface that will have an instance created for each move for each character.
    //Not 100% on grab implementation yet but will hopefully update this comment in the future

    string characterName { get; }

    int health { get; set; }

    //Walking
    double forwardWalkSpeed { get; }
    double backwardWalkSpeed { get; }

    //Dashes
    double forwardDashDistance { get; }
    double backDashDistance { get; }

    //Jumping
    double forwardJumpDistance { get; }
    double backwardJumpDistance { get; }

}
