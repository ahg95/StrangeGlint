using NUnit.Framework;
using UnityEngine;

public class Vector2UtilityTest
{
    [Test]
    public void CalculateLinePointClosestToOriginTest() {

        // Test a bunch of random lines.
        for (int i = 0; i < 10; i++)
        {
            // Construct the solution first.
            var x = (Random.value - 0.5f) * 200;
            var y = (Random.value - 0.5f) * 200;

            var solution = new Vector2(x, y);

            // Then construct the lineDirection that makes this point a solution.
            var lineDirection = Vector2.Perpendicular(solution).normalized;

            // Construct the line origin
            var lineOrigin = solution + (Random.value - 0.5f) * lineDirection;

            // Give the perpendicular direction a random length.
            lineDirection = (Random.value - 0.5f) * 200 * lineDirection;

            var point = Vector2Utility.CalculateLinePointClosestToOrigin(lineOrigin, lineDirection);

            var difference = (solution - point).magnitude;

            Assert.Less(difference, 0.001f);
        }

        // Test specific lines which could be a problem.
        for (int i = 0; i < 10; i++)
        {
            var solution = Vector2.zero;

            var x = (Random.value - 0.5f) * 200;
            var y = (Random.value - 0.5f) * 200;

            var lineDirection = new Vector2(x, y);

            var point = Vector2Utility.CalculateLinePointClosestToOrigin(solution, lineDirection);

            Assert.Less(point.magnitude, 0.001f);
        }
    }

}
