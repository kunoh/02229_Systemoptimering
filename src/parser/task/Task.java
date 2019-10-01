package parser.task;

public class Task {

    public String Id, Name;

    public int Wcet, Period, Deadline;

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }

    public int getWcet() {
        return Wcet;
    }

    public void setWcet(int wcet) {
        Wcet = wcet;
    }

    public int getPeriod() {
        return Period;
    }

    public void setPeriod(int period) {
        Period = period;
    }

    public int getDeadline() {
        return Deadline;
    }

    public void setDeadline(int deadline) {
        Deadline = deadline;
    }
}
