using UnityEngine;
using System.Collections.Generic;

public class ThreadRope
{
    public class Node
    {
        public Vector3 current;
        public Vector3 previous;

        public Node(Vector3 pos)
        {
            current = previous = pos;
        }
    }

    public List<Node> nodes = new List<Node>();

    public float segmentLength = 0.08f;
    public int iterations = 10;

    public float elasticity = 0.15f;   // độ đàn hồi
    public float softness = 0.75f;     // 1 = rất mềm, 0.3 = hơi cứng
    public float damping = 0.96f;      // giảm rung

    public Vector3 gravity = new Vector3(0, -0.4f, 0);

    public void Initialize(Vector3 startPoint, int count, float segLength)
    {
        nodes.Clear();
        segmentLength = segLength;

        for (int i = 0; i < count; i++)
        {
            nodes.Add(new Node(startPoint));
        }
    }

    public void Simulate(Vector3 anchor, Vector3 tip)
    {
        int last = nodes.Count - 1;

        nodes[0].current = anchor;
        nodes[last].current = tip;

        // ---- Verlet Integration ----
        for (int i = 1; i < last; i++)
        {
            Node n = nodes[i];

            Vector3 velocity = (n.current - n.previous) * damping;

            n.previous = n.current;
            n.current += velocity;
            n.current += gravity * Time.deltaTime;
        }

        // ---- Constraints + Elasticity ----
        for (int iter = 0; iter < iterations; iter++)
        {
            ApplyConstraints(anchor, tip);
        }
    }

    private void ApplyConstraints(Vector3 anchor, Vector3 tip)
    {
        int last = nodes.Count - 1;

        nodes[0].current = anchor;
        nodes[last].current = tip;

        for (int i = 0; i < last; i++)
        {
            Node a = nodes[i];
            Node b = nodes[i + 1];

            Vector3 delta = b.current - a.current;
            float dist = delta.magnitude;

            float target = segmentLength;

            // allow elasticity
            target += Mathf.Sin(Time.time * 8f + i * 0.4f) * elasticity * 0.03f;

            float diff = (dist - target) / dist;

            // softness (0 = hard rope, 1 = very soft rope)
            float adjust = diff * softness;

            if (i != 0)
                a.current += delta * adjust * 0.5f;

            if (i != last - 1)
                b.current -= delta * adjust * 0.5f;
        }
    }
}
