package parser.task;

import java.util.ArrayList;
import java.util.List;

public class Task {

    private String Id, Name;

    private int Wcet, Period, Deadline;

    private List<Edge> Edges;

    public Task(String id, String name, int wcet, int period, int deadline) {
        this.Id = id;
        this.Name = name;
        this.Wcet = wcet;
        this.Period = period;
        this.Deadline = deadline;
        this.Edges = new ArrayList<>();
    }

    public void addEdge(Edge edge) {
        this.Edges.add(edge);
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
