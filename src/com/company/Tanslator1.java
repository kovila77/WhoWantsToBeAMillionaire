package com.company;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.sql.*;
import java.util.ArrayList;

class Tanslator1 {

   public static void Translate() {

        ArrayList<Question> questions = ReadFile();

        try {
            Class.forName("org.sqlite.JDBC");
            Connection conn = DriverManager.getConnection("jdbc:sqlite:C:\\Users\\35498\\IdeaProjects\\WhoWantsToBeAMillionaire\\bilion.sqlite");

            PreparedStatement statmt = conn.prepareStatement(
                    "insert into questions (text, answer_1, answer_2, answer_3, answer_4, right_answer, level)\n" +
                        "values (?, ?, ?, ?, ?, ?, ?);");

            for (var q : questions) {
                statmt.setString(1, q.Text);
                statmt.setString(2, q.Answers[0]);
                statmt.setString(3, q.Answers[1]);
                statmt.setString(4, q.Answers[2]);
                statmt.setString(5, q.Answers[3]);
                statmt.setString(6, q.RightAnswer);
                statmt.setInt(7, q.Level);

                System.out.println(q.Text);

                statmt.executeUpdate();
                //ResultSet resSet = statmt.executeQuery();
            }

            conn.close();
        } catch (Exception ex) {
            System.out.println(ex.toString());
        }
    }

    static private ArrayList<Question> ReadFile() {
        ArrayList<Question> questions = new ArrayList<Question>();

        try {
            FileInputStream fstream = new FileInputStream("C:\\Users\\35498\\IdeaProjects\\WhoWantsToBeAMillionaire\\Вопросы.txt");
            BufferedReader br = new BufferedReader(new InputStreamReader(fstream));
            String strLine;

            while ((strLine = br.readLine()) != null) {
                String[] s = strLine.split("\t");
                questions.add(new Question(s));
            }
        } catch (IOException e) {
            System.out.println("Ошибка");
            return null;
        }

        return questions;
    }
}
