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
        if (this.transform.localPosition.y < 0)
        {
            this._rBody.angularVelocity = Vector3.zero;
            this._rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        }

        // ターゲット位置リセット
        target.localPosition = new Vector3(
            Random.value * 8 - 4, 0.5f, Random.value * 8 - 4
        );
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> vectorAction = actions.DiscreteActions;

        // 行動（vectorAction）の内容に応じて、rBody経由でRollerAgentへ力を加える
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int action = vectorAction[0];
        if (action == 1) dirToGo = transform.forward;
        if (action == 3) rotateDir = transform.up * -1.0f;
        if (action == 2) dirToGo = transform.forward * -1.0f;
        if (action == 4) rotateDir = transform.up;

        this.transform.Rotate(rotateDir, Time.deltaTime * 200f);
        _rBody.AddForce(dirToGo * 0.4f, ForceMode.VelocityChange);

        float distanceToTarget = Vector3.Distance(
            this.transform.localPosition, target.localPosition
        );
        // targetとの距離が一定以内であれば報酬獲得
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // Roller Agent落下時は報酬無しでエピソード終了
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    // ヒューリスティックモードの行動時に実行
    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        ActionSegment<float> actionsOut = actionBuffers.ContinuousActions;

        actionsOut[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow)) actionsOut[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow)) actionsOut[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow)) actionsOut[0] = 3;
        if (Input.GetKey(KeyCode.RightArrow)) actionsOut[0] = 4;
    }
}
