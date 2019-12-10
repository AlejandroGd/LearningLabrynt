using System.Collections;
using System.Collections.Generic;

public class QLearningAI 
{
    //Q-matrix with the learning values.
    private QMatrix qMat;

    //Q-Learning algorithm parameters.
    private double alpha, gamma, epsilon, epsilonDecay1, epsilonDecay2;

    //Constructor.
    public QLearningAI(double alpha, double gamma, double epsilon, 
                       double epsilonDecay1, double epsilonDecay2)
    {
        this.alpha = alpha;
        this.gamma = gamma;
        this.epsilon = epsilon;
        this.epsilonDecay1 = epsilonDecay1;
        this.epsilonDecay2 = epsilonDecay2;

        qMat = new QMatrix();
    }
}


