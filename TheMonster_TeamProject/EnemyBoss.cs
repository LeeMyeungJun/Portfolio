using System;
using System.Collections.Generic;
using UnityEngine;



public class EnemyBoss : Enemy
{
    [SerializeField]
    protected List<AnimData> detectAnimDatas = new List<AnimData>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        

    }
    protected override INode SettingBT()
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
                            new ActionNode(DoWait),
                        }
                    ),
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(CheckDetectEQS),
                            new ActionNode(MoveToEqsPoint),
                            new ActionNode(DoRangeAttack),
                            new ActionNode(DoWait),
                        }
                    ),
                    new ActionNode(MoveToDetectEnemy),
                }
            );
    }

    protected override void Update()
    {
        base.Update();
    }



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
    #region Range Attack Node
    INode.ENodeState CheckRangeAttacking()
    {
        if (IsAnimationRunning(curAnimData.animStateName))
        {
            return INode.ENodeState.Running;
        }

        return INode.ENodeState.Success;
    }

    INode.ENodeState CheckEnemyWithinRangeAttackRange()
    {
        if (_detectedPlayer != null)
        {
            if (Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < (_rangeAttackRange * _rangeAttackRange))
            {
                return INode.ENodeState.Success;
            }
        }
        return INode.ENodeState.Failure;
    }

    INode.ENodeState DoRangeAttack()
    {
        if (_detectedPlayer != null)
        {
            agent.isStopped = false;
            agent.velocity = (_detectedPlayer.position - transform.position).normalized * 25;
            agent.destination = _detectedPlayer.position;

            int ran = UnityEngine.Random.Range(0, rangeAnimDatas.Count);
            curAnimData = rangeAnimDatas[ran];
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
        if (_detectedPlayer != null)
            return INode.ENodeState.Success;
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
            //transform.position = Vector3.MoveTowards(transform.position, _detectedPlayer.position, Time.deltaTime * _movementSpeed);

            return INode.ENodeState.Running;
        }

        return INode.ENodeState.Failure;
    }
    #endregion
    #region  DetectEQS & EQSMove Node
    INode.ENodeState CheckDetectEQS()
    {
        if (!default(EnvironmentQuery.EQSData).Equals(eqs))
            return INode.ENodeState.Success;

        eqs = EnvironmentQuery.GetEqsRandomHighPoint(16, _rangeAttackRange, _rangeAttackRange / 3, this.gameObject, agent);

        if (Vector3.SqrMagnitude(eqs.position - transform.position) < (_rangeAttackRange * _rangeAttackRange))
            return INode.ENodeState.Success;

        return INode.ENodeState.Failure;
    }
    INode.ENodeState MoveToEqsPoint()
    {
        if (!default(EnvironmentQuery.EQSData).Equals(eqs))
        {
            if (Vector3.Distance(eqs.position, transform.position) < 1.5f)
            {
                eqs = default(EnvironmentQuery.EQSData);
                return INode.ENodeState.Success;
            }
            if (agent.isStopped)
                agent.isStopped = false;

            agent.destination = eqs.position;
            //transform.position = Vector3.MoveTowards(transform.position, _detectedPlayer.position, Time.deltaTime * _movementSpeed);

            return INode.ENodeState.Running;
        }

        return INode.ENodeState.Failure;
    }

    #endregion

}
