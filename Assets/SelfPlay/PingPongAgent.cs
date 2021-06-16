using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PingPongAgent : Agent
{
    public int agentId;
    public GameObject ball;
    private Rigidbody ballRb;

    public override void Initialize()
    {
        this.ballRb = this.ball.GetComponent<Rigidbody>();
    }

    // 観察時に実行
    public override void CollectObservations(VectorSensor sensor)
    {
        float dir = (agentId == 0) ? 1.0f : -1.0f;
        // ボールx座標
        sensor.AddObservation(this.ball.transform.localPosition.x * dir);
        // ボールZ座標
        sensor.AddObservation(this.ball.transform.localPosition.z * dir);
        // ボールX速度
        sensor.AddObservation(this.ballRb.velocity.x * dir);
        // ボールZ速度
        sensor.AddObservation(this.ballRb.velocity.z * dir);

        // パドルのX座標
        sensor.AddObservation(this.transform.localPosition.x * dir);
    }

    // ボールとパドルの衝突時に実行
    private void OnCollisionEnter(Collision collision)
    {
        AddReward(0.1f);
    }

    // 行動時に実行
    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> vectorAction = actions.DiscreteActions;

        float dir = (agentId == 0) ? 1.0f : -1.0f;
        int action = vectorAction[0];
        Vector3 pos = this.transform.localPosition;

        // ポリシーから受け取ったアクションによりパドルの移動方向が変わる
        if (action == 1)
        {
            pos.x -= 0.2f * dir;
        }
        else if (action == 2)
        {
            pos.x += 0.2f * dir;
        }

        // パドルがはみ出そうになったらそれ以上移動できないようにする
        if (pos.x < -4.0f)
        {
            pos.x = -4.0f;
        }

        if (pos.x > 4.0f)
        {
            pos.x = 4.0f;
        }

        this.transform.localPosition = pos;
    }

    // ヒューリスティックモードのときに実行
    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        var actionsOut = actionBuffers.DiscreteActions;

        actionsOut[0] = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            actionsOut[0] = 1;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            actionsOut[0] = 2;
        }
    }
}
