using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BehaviourAI
{
    protected Transform _detectedPlayer = null;
    Vector3 originPos = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        _BTRunner = new BehaviorTree(SettingBT());
        _detectedPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        originPos = transform.position;
    }
    protected virtual INode SettingBT()
    {
        return new SelectorNode
            (
                new List<INode>()
                {
                    //new ActionNode(FollowTarget),
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(CheckMeleeAttacking),
                            new ActionNode(CheckEnemyWithinMeleeAttackRange),
                            new ActionNode(DoMeleeAttack),
                            new ActionNode(DotTwoSecWait),
                        }
                    ),
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(CheckDetectEnemy),
                            new ActionNode(MoveToDetectEnemy),
                        }
                    ),
                    new ActionNode(MoveToOriginPosition)
                }
            );
    }

    protected override void Update()
    {
        base.Update();
    }

    #region Wait FollowTarget
    protected INode.ENodeState DoWait()
    {
        SetWaitTime(1.5f);
        return INode.ENodeState.Success;
    }
    protected INode.ENodeState DotTwoSecWait()
    {
        SetWaitTime(2f);
        return INode.ENodeState.Success;
    }
    protected INode.ENodeState FollowTarget()
    {
        if (_detectedPlayer != null)
        {
            Vector3 dir = _detectedPlayer.transform.position - this.transform.position;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10.5f);
        }
        return INode.ENodeState.Success;
    }
    #endregion
    #region Melee Attack Node
    INode.ENodeState CheckMeleeAttacking()
    {
        if (IsAnimationRunning(curAnimData.animStateName))
        {
            return INode.ENodeState.Running;
        }

        return INode.ENodeState.Success;
    }

    INode.ENodeState CheckEnemyWithinMeleeAttackRange()
    {
        if (_detectedPlayer != null)
        {
            if (Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < (_meleeAttackRange * _meleeAttackRange))
            {
                return INode.ENodeState.Success;
            }
        }

        return INode.ENodeState.Failure;
    }

    INode.ENodeState DoMeleeAttack()
    {
        if (_detectedPlayer != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            int ran = UnityEngine.Random.Range(0, atkAnimDatas.Count);
            curAnimData = atkAnimDatas[ran];
            _animator.SetTrigger(curAnimData.triggerName);
            transform.LookAt(_detectedPlayer);
            return INode.ENodeState.Success;
        }

        return INode.ENodeState.Failure;
    }
    #endregion
    #region  Detect & Move Node
    INode.ENodeState CheckDetectEnemy()
    {
        var overlapColliders = Physics.OverlapSphere(transform.position, _detectRange, LayerMask.GetMask("Player"));

        if (overlapColliders != null && overlapColliders.Length > 0)
        {
            _detectedPlayer = overlapColliders[0].transform;

            return INode.ENodeState.Success;
        }

        //_detectedPlayer = null;

        return INode.ENodeState.Failure;
    }

    INode.ENodeState MoveToDetectEnemy()
    {
        if (_detectedPlayer != null)
        {
            if (Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < (_meleeAttackRange * _meleeAttackRange))
            {
                return INode.ENodeState.Success;
            }
            if (agent.isStopped)
                agent.isStopped = false;

            agent.destination = _detectedPlayer.position;

            return INode.ENodeState.Running;
        }

        return INode.ENodeState.Failure;
    }
    #endregion
    #region  Move Origin Pos Node InIt
    INode.ENodeState MoveToOriginPosition()
    {
        if (Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < float.Epsilon * float.Epsilon)
        {
            return INode.ENodeState.Success;
        }
        else
        {
            agent.destination = originPos;
            return INode.ENodeState.Running;
        }
    }

    #endregion

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _detectRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, _rangeAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, _meleeAttackRange);
    }
}
