[System.Serializable]
public class Result{
    public string name;
    public int total_clicks;
    public int total_time;
    public int pairs;
    public int score;

    public Result(string name, int total_clicks, int total_time, int pairs, int score) {
        this.name = name;
        this.total_clicks = total_clicks;
        this.total_time = total_time;
        this.pairs = pairs;
        this.score = score;
    }
}