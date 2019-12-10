using System.Collections;
using System.Collections.Generic;

public enum Movement { UP, DOWN, RIGHT }

/**
 * This class contains the set of "rules" for the labrynt.
 * Implements the transition function, which defines actions (moves) avaliable on each state 
 * (cell) and new state (cell) after taking the action.
 * It also implements the reward matrix, which defines the rewards given for each pair of 
 * current state (cell) and action taken (movement).
 */
public static class LabryntRules 
{   
    //Return a list with the available movements for the player from the cell specified.
    public static List<Movement> GetPossibleMovements(int cell)
    {
        List<Movement> moves = new List<Movement>();

        if (cell == 11 || cell == 23 || cell == 35 || cell == 47 || cell == 59 || cell == 71 || cell == 83) return moves;

        //All cells after discarding the right column have a RIGHT movement
        moves.Add(Movement.RIGHT);

        //Any cell that is not in the last row (apart from the above) can move up
        if (cell < 71) moves.Add(Movement.DOWN);

        //Any cell that is not in the first row (apart from the above) can move down
        if (cell > 11) moves.Add(Movement.UP);

        return moves;
    }

    //Defines which cells are a final state
    public static bool IsFinalState(int cell)
    {
        if (cell == 11 || cell == 23 || cell == 35 || cell == 47 || cell == 59 || cell == 71 || cell == 83) return true;
        else return false;
    }

    public static int GetLandingCell(int startingCell, Movement move)
    {
        //Check cells without movement
        if (startingCell == 11 || startingCell == 23 || startingCell == 35 || startingCell == 47 || startingCell == 59 || startingCell == 71 || startingCell == 83) return -1;
        if (move == Movement.RIGHT) return startingCell + 1;
        if (startingCell > 11 && move == Movement.UP) return startingCell - 11;
        if (startingCell < 71 && move == Movement.DOWN) return startingCell + 13;

        //Any other case is not valid.
        return -1;
    }

    //Gets the reward obtained from choosing a movement in a determined cell.
    public static int GetReward(int cell, Movement move)
    {
        int landingCell = GetLandingCell(cell, move);

        switch(landingCell)
        {
            //Positive reward (Yellow circle) + 10
            case 8:
            case 9:
            case 19:
            case 22:
            case 30:
            case 35:
            case 49:
            case 51:
            case 59:
            case 66:
            case 81:
                return 10;
            //Negative Rewards (Red circle) - 10.
            case 4:
            case 5:
            case 10:
            case 16:
            case 17:
            case 25:
            case 28:
            case 29:
            case 33:
            case 40:
            case 41:
            case 50:
            case 52:
            case 53:
            case 56:
            case 62:
            case 82:
                return -10;
            //Any other case    
            default:
                return 0;
        }
    }    
}
