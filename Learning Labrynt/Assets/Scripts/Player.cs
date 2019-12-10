using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] ValueSelector speedSelector;
    [SerializeField] ValueSelector alphaSelector;
    [SerializeField] ValueSelector gammaSelector;
    [SerializeField] Dropdown epsd1Selector;
    [SerializeField] Dropdown epsd2Selector;
    [SerializeField] LabryntDisplay labryntDisplay;
    [SerializeField] TextMeshProUGUI episodeNumberText;
    [SerializeField] TextMeshProUGUI epsilonValueText;

    bool paused;
    bool restart = true;
    bool newEpisode = true;
    int episodeCounter;
    int currentCell;
    private QMatrix qMat; //Q-matrix with the learning values.

    //Q-Learning algorithm parameters.

    private double alpha;
    //Alpha (Learning rate). Affects how much the agent (the player) learns. 
    //Larger values allow to “override” the q-matrix with new information faster but take longer for the values to stabilise as new 
    //information can easily change the Q-matrix.
    //It can be seen as how much the new experiences affect the knowledge the player already posesses.

    private double gamma;
    //Gamma (Discount factor). Affects how the agent consider the value of future rewards versus immediate ones. Small values for gamma 
    //will make an agent avoid negative cells at all costs. Big values will consider passing through negative cells if that is the only way to get a bigger reward after.
    


    private double epsilon;
    private double epsilonDecay1;
    private double epsilonDecay2;




    int cumulativeReward;
    System.Random rnd; //Random number generator
    
    float timer;

    // Start is called before the first frame update
    void Start()
    {       
        paused = true;

        restart = true;
        newEpisode = true;

        //Initialise timer and put player in first position.
        timer = 0;

        rnd = new System.Random();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {           
                        
            timer += Time.deltaTime;
            float dt = speedSelector.GetInverse();
            if (timer > dt)
            {
                //Calculate how many choices we need according to the speed selector.
                int loops = (int)(timer / dt);
                while (loops > 0)
                {
                    if (restart)
                    {
                        InitialiseAlgorithm();
                        restart = false;
                        newEpisode = true;
                    }

                    if (newEpisode)
                    {
                        SetValuesForNewEpisode();
                        newEpisode = false;
                    }

                    Movement currentMove;
                    //Choose action (a) based on policy (p) 
                    if (ShouldExplore())
                    {
                        currentMove = GetRandomMovement(currentCell);
                    }
                    else
                    {
                        currentMove = GetBestMovement(currentCell);
                    }

                    //Observe reward (r) 
                    int reward = LabryntRules.GetReward(currentCell, currentMove);
                    double oldQValue = qMat.GetQValue(currentCell, currentMove);

                    //Do an estimation of the best Q-value to obtain from next state.
                    int nextCell = LabryntRules.GetLandingCell(currentCell, currentMove);
                    double estimation = CalculateEstimate(nextCell);

                    //Recalculate Q-value for the current cell based on estimation.
                    double updatedQValue = CalculateNewQValue(oldQValue, alpha, gamma, reward, estimation);
                    qMat.SetQValue(currentCell, currentMove, updatedQValue);

                    cumulativeReward += reward;
                    currentCell = nextCell;

                    UpdatePlayerPositionInDisplay(); //Now we update the display with the new player position.

                    //If the player landed in an final state cell, we need to update epsilon and start a new episode.
                    if (LabryntRules.IsFinalState(currentCell))
                    {
                        //Change epsilon parameter so it shifts from exploration to explotation gradually between episodes.
                        if (epsilon > 0.3) epsilon *= epsilonDecay1;
                        if (epsilon < 0.3) epsilon *= epsilonDecay2;
                        newEpisode = true;
                    }

                    loops--;
                }

                timer -= dt;
            }
        }
    }

    //Initialise the parameters for a new run of the Q-learning algorithm.
    private void InitialiseAlgorithm()
    {
        qMat = new QMatrix();
        alpha = alphaSelector.GetSliderValue();
        gamma = gammaSelector.GetSliderValue();
        epsilon = 0.99999999;
        epsilonDecay1 = System.Convert.ToDouble(epsd1Selector.captionText.text);
        epsilonDecay2 = System.Convert.ToDouble(epsd2Selector.captionText.text);
        episodeCounter = 0;
    }

    //Set the player in the initial position and clear any cumulative reward to start a new episode.
    private void SetValuesForNewEpisode()
    {
        episodeCounter++;
        cumulativeReward = 0;
        currentCell = 36;

        labryntDisplay.ClearTrail();        
        labryntDisplay.MovePlayer(currentCell);
        epsilonValueText.text = "Epsilon: " + epsilon;
        episodeNumberText.text = "Episode N: " + episodeCounter;
        
    }

    //Uses an epsilon-greedy policy to determine if the player decides its next move by exploring or exploiting.
    //The epsilon value is initially high, which will favour exploring (choosing the next move randomly). With each episode,
    //epsilon value decreases, which will make the player shifting gradually to exploiting, (using the knowledge registered in 
    //the Q-matrix to decide the best move).
    private bool ShouldExplore()
    {
        bool explore = false;
        double randomNumber = rnd.NextDouble();
        if (randomNumber < epsilon) explore = true;

        return explore;
    }

    //Check available movements from a cell and returns one of them randomly.
    private Movement GetRandomMovement(int fromCell)
    {
        List<Movement> possibleMoves = LabryntRules.GetPossibleMovements(fromCell);
        int randomIndex = Random.Range(0, possibleMoves.Count);
        return possibleMoves[randomIndex];
    }

    //Checks the Q-Matrix and returns the best move from the available ones.
    //Best movement is decided by choosing the one with the highest q-value. If the highest value appears in more 
    //than one move, chooses randomly between those moves
    private Movement GetBestMovement(int fromCell)
    {
        List<Movement> bestMovements = new List<Movement>();
        List<Movement> possibleMovements = LabryntRules.GetPossibleMovements(fromCell);

        //There is at least one move from all cells (terminal states stop the algorithm before getting here)
        //so it is saved to compare with the rest.
        bestMovements.Add(possibleMovements[0]);
        double highestQValue = qMat.GetQValue(fromCell, possibleMovements[0]);

        //Compare to other values, keep a list with the movements with highest q-value.
        for (int x = 1; x < possibleMovements.Count; x++)
        {
            double qValue = qMat.GetQValue(fromCell, possibleMovements[x]);
            if (highestQValue == qValue)
            {
                bestMovements.Add(possibleMovements[x]);
            }
            else if (highestQValue < qValue)
            {
                bestMovements.Clear();
                bestMovements.Add(possibleMovements[x]);
                highestQValue = qValue;
            }
        }

        //If more than one has the highest value, choos randomly between them.
        int index = 0;
        if (bestMovements.Count > 1) index = Random.Range(0, bestMovements.Count);

        return bestMovements[index];
    }

    //Gives you the max Q-Value possible from a determined state (cell).
    private double CalculateEstimate(int nextCell)
    {
        double estimate = 0;

        if (!LabryntRules.IsFinalState(nextCell))
        {
            Movement bestMove = GetBestMovement(nextCell);
            estimate = qMat.GetQValue(nextCell, bestMove);
        }

        return estimate;
    }

    //Calculates the new Q-value for a cell given the reward obtained on it and  
    //the estimation for the best outcome in next cell.
    private double CalculateNewQValue(double oldQValue, double alpha, double gamma,
                                      double reward, double estimation)
    {
        double newQas = 0;

        //new Q(s,a) = Q(s,a) + alpha[ r + gamma * maxQ(s',a') - Q(s,a)]
        newQas = oldQValue + alpha * (reward + gamma * estimation - oldQValue);

        return newQas;
    }

    //Updates the display with the new player position.
    private void UpdatePlayerPositionInDisplay()
    {        
        labryntDisplay.MovePlayer(currentCell);
    }


    public void TogglePause()
    {
        paused = !paused;
    }

    public void Restart()
    {
        restart = true;
        newEpisode = true;
        paused = true;
        qMat = new QMatrix();        
        currentCell = 36;
        labryntDisplay.ClearTrail();
        labryntDisplay.MovePlayer(currentCell);
    }
   

}

//Q-Matrix containing all Q-values for a given state (cell) and action taken
public class QMatrix
{
    private double[,] matrix; //Q-Matrix [ state, action ]

    public QMatrix()
    {
        //All values initialised to 0 by default in C#
        matrix = new double[84, 3]; //24 cells, 3 possible actions (UP / RIGHT / DOWN)
    }

    public void SetQValue(int cell, Movement move, double value)
    {
        switch (move)
        {
            case Movement.UP:
                matrix[cell, 0] = value;
                break;
            case Movement.RIGHT:
                matrix[cell, 1] = value;
                break;
            case Movement.DOWN:
                matrix[cell, 2] = value;
                break;
        }
    }

    public double GetQValue(int cell, Movement move)
    {
        switch (move)
        {
            case Movement.UP:
                return matrix[cell, 0];
            case Movement.RIGHT:
                return matrix[cell, 1];
            case Movement.DOWN:
                return matrix[cell, 2];
            default:
                return 0;
        }
    }
}