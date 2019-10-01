package parser.task;

import java.util.List;

public class Task {

    private String Id, Name;

    private int Wcet, Period, Deadline;

    private List<Edge> Edges;

    public Task() {
    }

    public String getId() {
        return Id;
    }

    public void setId(String id) {
        Id = id;
    }

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
