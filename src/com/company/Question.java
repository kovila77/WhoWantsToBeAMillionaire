package com.company;

public class Question {

    public Question(String Text, String Answer1, String Answer2, String Answer3, String Answer4, String RightAnswer, int Level) {
        this.Text = Text;
        this.Answers[0] = Answer1;
        this.Answers[1] = Answer2;
        this.Answers[2] = Answer3;
        this.Answers[3] = Answer4;
        this.RightAnswer = RightAnswer;
        this.Level = Level;

    }

    public Question(String[] s) {
        Text = s[0];
        for (int i = 0; i < 4; i++)
            Answers[i] = s[i + 1];
        RightAnswer = s[5];
        Level = Integer.parseInt(s[6]);
    }

    public String Text;
    public String[] Answers = new String[4];
    public String RightAnswer;
    public int Level;
}
