package parser.task;

public class Edge {

    private String Source;

    private Task Dest;

    private int Cost;

    public String getSource() {
        return Source;
    }

    public void setSource(String source) {
        Source = source;
    }

    public Task getDest() {
        return Dest;
    }

    public void setDest(Task dest) {
        Dest = dest;
    }

    public int getCost() {
        return Cost;
    }

    public void setCost(int cost) {
        Cost = cost;
    }
}
