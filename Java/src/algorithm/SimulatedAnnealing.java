package algorithm;

public class SimulatedAnnealing {

    private double temp, coolingRate;

    // private Plan currentPlan, bestPlan;

    public SimulatedAnnealing(double initTemp, double coolingRate) {
        this.temp = initTemp;
        this.coolingRate = coolingRate;
    }

    public void simulate() {
        // Create random initial plan
        // currentPlan = new Plan();
        // currentPlan.generateRandomPlan();

        // Keep track of best plan
        // bestPlan = currentPlan;

        while(temp > 1) {
            // Plan newPlan = new Plan(currentPlan);

            // Get two random tasks in plan

            // Ensure tasks are not the same task

            // Swap the tasks

            // Determine energy of newPlan and currentPlan

            // Decide which plan should be saved for next iteration

            // Save the better of the plans, in case no better plan is ever found

            // Cool the system
            temp *= 1 - coolingRate;
        }
    }
}
