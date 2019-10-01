package parser;

import java.util.HashMap;
import java.util.Map;

import parser.task.Task;

public class Main {

    public static void main(String[] args) {
    		
    	TaskParser tp = new TaskParser();
    	tp.parseXml("test\\Selected exercise test cases\\Case 3\\Case3.tsk");
    	Map<String, Task> tasks = tp.getTasks();
    	
    }
}