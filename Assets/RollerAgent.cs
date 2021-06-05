using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{
    public Transform target;

    private Rigidbody _rBody;

    public override void Initialize()
    {
        this._rBody = GetComponent<Rigidbody>();
    }

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
        // Roller Agent落下時
        if (this.transform.position.y < 0)
        {
            this._rBody.angularVelocity = Vector3.zero;
            this._rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        }

        // ターゲット位置リセット
        target.position = new Vector3(
            Random.value * 8 - 4, 0.5f, Random.value * 8 - 4
        );
    }

    // 観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(_rBody.velocity.x);
        sensor.AddObservation(_rBody.velocity.z);
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> vectorAction = actions.ContinuousActions;

        // 行動（vectorAction）の内容に応じて、rBody経由でRollerAgentへ力を加える
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        _rBody.AddForce(controlSignal * 10);

        float distanceToTarget = Vector3.Distance(
            this.transform.position, target.position
        );
        // targetとの距離が一定以内であれば報酬獲得
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // Roller Agent落下時は報酬無しでエピソード終了
        if (this.transform.position.y < 0)
        {
            EndEpisode();
        }
    }

    // ヒューリスティックモードの行動時に実行
    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        ActionSegment<float> actionsOut = actionBuffers.ContinuousActions;

        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
