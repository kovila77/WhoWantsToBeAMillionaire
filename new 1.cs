package com.company;

import org.w3c.dom.ranges.Range;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.WindowEvent;
import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.*;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

public class GameFrame extends JFrame {
    private JPanel panel1;
    private JButton btnAnswer3;
    private JButton btnAnswer1;
    private JButton btnAnswer2;
    private JButton btnAnswer4;
    private JList lstLevel;
    private JButton btHalfHalf;
    private JButton btHelpAudience;
    private JButton btHelpFriend;
    private JButton btTakeMoney;
    private JLabel lblQuestionText;
    private JButton btCanError;
    private JButton btChangeQ;

    ArrayList<Question> questions = new ArrayList<Question>();
    DefaultComboBoxModel<String> comboBoxModel = new DefaultComboBoxModel<>();
    JComboBox jcb = new JComboBox(comboBoxModel);
    private Random rnd = new Random();
    int Level = 0;
    private int tip = 0;
    private Boolean canError = false;
    Question currentQuestion;
    private int selectedIndex;
    private int hardSum;

    public GameFrame() {
        super("hello world");


        //comboBoxModel.addAll(Arrays.asList("1000","500"));
        comboBoxModel.addAll(Arrays.asList("3 000 000", "1 500 000", "800 000", "400 000", "200 000", "100 000",
                "50 000", "25 000", "15 000", "10 000", "5 000", "3 000", "2 000", "1 000", "500"));
        lstLevel.setModel(comboBoxModel);

        setContentPane(panel1);
        this.setMinimumSize(new Dimension(800, 500));

        pack();
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        //ReadFile();
        seadDb();
        this.setVisible(true);
        startGame();

        btnAnswer1.addActionListener(this::bntAnswerPerformed);
        btnAnswer2.addActionListener(this::bntAnswerPerformed);
        btnAnswer3.addActionListener(this::bntAnswerPerformed);
        btnAnswer4.addActionListener(this::bntAnswerPerformed);


        btTakeMoney.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                pickUpAPrize();
            }
        });

        btHalfHalf.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                bntFiftyFiftyActionPerformed(e);
                tip++;
                btHalfHalf.setEnabled(false);
                if (tip > 2) setEnabledTipButton(false);
                lstLevel.repaint();
            }
        });

        btHelpAudience.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                int[] ii = {rnd.nextInt(40), rnd.nextInt(40), rnd.nextInt(40)};
                int index = 0;
                int rightA = Math.max((100 - IntStream.of(ii).sum()), 0);

                String response = "Результаты голосования: ";
                for (int j = 1; j < 5; j++) {
                    if (Integer.parseInt(currentQuestion.RightAnswer.trim()) == j) {
                        response += " за " + j + "-" + rightA + " голосов,";
                    } else {
                        response += " за " + j + "-" + ii[index++] + " голосов,";
                    }
                }

                JOptionPane.showMessageDialog((Component) e.getSource(), response);
                btHelpAudience.setEnabled(false);
                tip++;
                if (tip > 2) setEnabledTipButton(false);
                lstLevel.repaint();
            }
        });

        btHelpFriend.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                boolean b = Math.max(rnd.nextInt(100), 0) > 50;
                String response = "Друг советует ";
                if (b) {
                    JOptionPane.showMessageDialog((Component) e.getSource(), 
                            response + currentQuestion.RightAnswer.trim());
                } else {
                    JOptionPane.showMessageDialog((Component) e.getSource(), 
                            response + (rnd.nextInt(4) + 1));
                }

                btHelpFriend.setEnabled(false);
                tip++;
                if (tip > 2) setEnabledTipButton(false);
                lstLevel.repaint();
            }
        });

        btCanError.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                canError = true;
                tip++;
                btCanError.setEnabled(false);
                if (tip > 2) setEnabledTipButton(false);
                lstLevel.repaint();
            }
        });

        btChangeQ.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                Level--;
                selectedIndex++;
                NextStep();
                tip++;
                btChangeQ.setEnabled(false);
                if (tip > 2) setEnabledTipButton(false);
                lstLevel.repaint();
            }
        });


        lstLevel.setCellRenderer(new SelectedListCellRenderer());
    }

    public class SelectedListCellRenderer extends DefaultListCellRenderer {
        @Override
        public Component getListCellRendererComponent(JList list, Object value, 
                                                      int index, boolean isSelected, boolean cellHasFocus) {
            Component c = super.getListCellRendererComponent(list, value, index, isSelected, cellHasFocus);
            if (index == selectedIndex) {
                c.setBackground(Color.YELLOW);
            } else if (index == hardSum) {
                c.setBackground(Color.RED);
            } else {
                c.setBackground(Color.WHITE);
            }

            return c;
        }
    }

    private void bntAnswerPerformed(java.awt.event.ActionEvent evt) {

        if (currentQuestion.RightAnswer.equals(evt.getActionCommand()))
            NextStep();
        else {
            if (canError) {
                ((JButton) evt.getSource()).setEnabled(false);
                canError = false;
            } else {
                JOptionPane.showMessageDialog(this, "Неверный ответ!");

                if (selectedIndex < hardSum)
                    JOptionPane.showMessageDialog(this,
                            "Ваш приз: " + comboBoxModel.getElementAt(hardSum).trim()
                                    + "\n*протягивает кейс с деньгами*");

                startGame();
            }
        }
        canError = false;
    }

    private void bntFiftyFiftyActionPerformed(java.awt.event.ActionEvent evt) {
        JButton[] buttons = new JButton[]{btnAnswer1, btnAnswer2,
                btnAnswer3, btnAnswer4};

        int count = 0;
        while (count < 2) {
            int n = rnd.nextInt(4);
            String ac = buttons[n].getActionCommand();

            if (!ac.equals(currentQuestion.RightAnswer)
                    && buttons[n].isEnabled()) {
                buttons[n].setEnabled(false);
                count++;
            }
        }
    }

    private void seadDb() {
        try {
            Class.forName("org.sqlite.JDBC");
            String conStr = "jdbc:sqlite:C:\\Users\\35498\\IdeaProjects\\WhoWantsToBeAMillionaire\\bilion.sqlite";
            Connection conn = DriverManager.getConnection(conStr);

            Statement statmt = conn.createStatement();
            String query = "select text,\n" +
                    "       answer_1,\n" +
                    "       answer_2,\n" +
                    "       answer_3,\n" +
                    "       answer_4,\n" +
                    "       right_answer,\n" +
                    "       level\n" +
                    "from questions";

            ResultSet resSet = statmt.executeQuery(query);

            while (resSet.next()) {
                String text = resSet.getString(1);
                String a1 = resSet.getString(2);
                String a2 = resSet.getString(3);
                String a3 = resSet.getString(4);
                String a4 = resSet.getString(5);
                String ra = resSet.getString(6);
                int lvl = resSet.getInt(7);

                questions.add(new Question(text, a1, a2, a3, a4, ra, lvl));
            }

            resSet.close();
            conn.close();
        } catch (Exception ex) {
            System.out.println(ex.toString());
        }
    }

    private void ShowQuestion(Question q) {
        lblQuestionText.setText(q.Text);
        btnAnswer1.setText(q.Answers[0]);
        btnAnswer2.setText(q.Answers[1]);
        btnAnswer3.setText(q.Answers[2]);
        btnAnswer4.setText(q.Answers[3]);
    }

    private Question GetQuestion(int level) {
        java.util.List<Question> list =
                questions.stream().filter(q -> q.Level == level).collect(Collectors.toList());
        return list.get(rnd.nextInt(list.size()));
    }

    private void NextStep() {
        JButton[] btns = new JButton[]{btnAnswer1, btnAnswer2,
                btnAnswer3, btnAnswer4};

        for (JButton btn : btns)
            btn.setEnabled(true);

        Level++;
        currentQuestion = GetQuestion(Level);
        ShowQuestion(currentQuestion);

        //int t = comboBoxModel.getSize() - Level;

        if (Level > 1) {
            btTakeMoney.setEnabled(true);
        }
        selectedIndex--;
        if (selectedIndex < 0) {
            JOptionPane.showMessageDialog(this, "Вы выйграли максимум денег!");
            pickUpAPrize();
        }
        lstLevel.repaint();
    }

    private void startGame() {
        Level = 0;
        tip = 0;

        canError = false;
        setEnabledTipButton(true);

        jcb.setSelectedIndex(comboBoxModel.getSize() - 1);
        JOptionPane.showMessageDialog(null, 
                jcb, "Выберете несгараемую сумму", JOptionPane.QUESTION_MESSAGE);
        hardSum = jcb.getSelectedIndex();
        selectedIndex = comboBoxModel.getSize();

        lstLevel.repaint();
        btTakeMoney.setEnabled(false);
        NextStep();
    }

    private void setEnabledTipButton(boolean enable) {
        btHalfHalf.setEnabled(enable);
        btHelpAudience.setEnabled(enable);
        btHelpFriend.setEnabled(enable);
        btCanError.setEnabled(enable);
        btChangeQ.setEnabled(enable);
    }

    private void pickUpAPrize() {
        JOptionPane.showMessageDialog(this, 
                "Ваш приз: " + comboBoxModel.getElementAt(selectedIndex + 1).trim()
                        + "\n*протягивает кейс с деньгами*");
        startGame();
    }
}
