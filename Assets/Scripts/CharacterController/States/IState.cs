using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void StateEnter();
    void StateExit();
    void StateUpdate();
}
