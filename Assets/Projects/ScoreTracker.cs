namespace Project
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ScoreTracker : MonoBehaviour
    {
        [SerializeField] private float checkInterval = 1f; // スコアを測る間隔（秒）
        [SerializeField] private List<Rigidbody2D> projectiles; // 対象の玉たち
        [SerializeField] ScoreView _scoreView;

        private float maxScore = 0f;
        private float timer = 0f;

        public float MaxScore => maxScore;

        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= checkInterval)
            {
                timer = 0f;
                float currentScore = CalculateScore(projectiles);

                if (currentScore > maxScore)
                {
                    maxScore = currentScore;
                    _scoreView.Set(maxScore);
                    //Debug.Log($"🎯 New Max Score: {maxScore}");
                }
            }
        }

        float CalculateScore(List<Rigidbody2D> projectiles)
        {
            if (projectiles.Count == 0) return 0f;

            var speeds = projectiles.Select(p => p.linearVelocity.magnitude).ToList();
            float speedStdDev = StandardDeviation(speeds);

            var directions = projectiles.Select(p => p.linearVelocity.normalized).ToList();
            Vector2 avgDir = Vector3.zero;
            foreach (var dir in directions) avgDir += dir;
            avgDir /= directions.Count;
            float directionVariance = 1f - avgDir.magnitude;

            int count = projectiles.Count;

            float score = (speedStdDev * 10f) + (directionVariance * 20f) + (count * 5f);
            return score;
        }

        float StandardDeviation(List<float> values)
        {
            if (values.Count == 0) return 0f;
            float avg = values.Average();
            float sumSq = values.Sum(v => (v - avg) * (v - avg));
            return Mathf.Sqrt(sumSq / values.Count);
        }

        public void Add(Rigidbody2D rb)
        {
            if (!projectiles.Contains(rb))
                projectiles.Add(rb);
        }

        public void Remove(Rigidbody2D rb)
        {
            projectiles.Remove(rb);
        }

        public void ClearProjectiles()
        {
            projectiles.Clear();
        }
    }
}