using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum Difficulty
{
    Bliss = 0,
    Normal = 1,
    Harder = 2,
}

public enum difficultyObject
{
    boss = 0,
    crow = 1,
    birb = 2,
}
public class difficultyManager : MonoBehaviour
{
    private Difficulty difficultylevel = Difficulty.Normal;
    public Difficulty getDifficulty()
    {
        return difficultylevel;
    }
    // object - difficulty - value
    private float[][][] difficultyValues = new float[3][][];
    private void initializeDifficulties()
    {
        // Boss: (speed, margin, stunTimer)
        difficultyValues[0] = new float[3][];
        difficultyValues[0][0] = new float[] { 3, 1.5f, 7 };
        difficultyValues[0][1] = new float[] { 5, 1.3f, 5 };
        difficultyValues[0][2] = new float[] { 7, 1.2f, 3 };
        // Crow: (speed, approachSpeed, scaredTimer, attackDistance)
        difficultyValues[1] = new float[3][];
        difficultyValues[1][0] = new float[] { 2, 2.5f, 12, 15 };
        difficultyValues[1][1] = new float[] { 2.55f, 3, 10, 10 };
        difficultyValues[1][2] = new float[] { 3, 4, 8, 8 };
        difficultyValues[2] = new float[3][];
        // Birb: ( totalFlaps, maxAmmo, startingLives )
        difficultyValues[2][0] = new float[] { 4, 6, 4 };
        difficultyValues[2][1] = new float[] { 3, 5, 3 };
        difficultyValues[2][2] = new float[] { 2, 3, 2 };

    }
    public void Awake()
    {
        initializeDifficulties();
    }

    public float[] getDifficultySettings(difficultyObject dif)
    {
        Difficulty temporalDifficulty = (SceneManager.GetActiveScene().buildIndex != (int)scene.tutorial) ? difficultylevel : Difficulty.Normal;
        float[] difficultySettings;
        difficultySettings = difficultyValues[(int)dif][(int)temporalDifficulty];
        return difficultySettings;
    }

    public void modifyDifficulty(int amount)
    {
        int dif = (int)difficultylevel + amount;
        if (dif > 2)
        {
            dif = 0;
        }
        if (dif < 0)
        {
            dif = 2;
        }
        difficultylevel = (Difficulty)dif;
    }
}
