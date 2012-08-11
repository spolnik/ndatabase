namespace Test.Odb.Test.VO.School
{
	public class History
	{
		private Discipline discipline;

		private Teacher teacher;

		private int score;

		private System.DateTime date;

		private Student student;

		public History()
		{
		}

		public History(System.DateTime data, Discipline discipline
			, int score, Teacher teacher)
		{
			this.date = data;
			this.discipline = discipline;
			this.score = score;
			this.teacher = teacher;
		}

		public virtual System.DateTime GetDate()
		{
			return date;
		}

		public virtual Discipline GetDiscipline()
		{
			return discipline;
		}

		public virtual int GetScore()
		{
			return score;
		}

		public virtual void SetDate(System.DateTime data)
		{
			this.date = data;
		}

		public virtual void SetDiscipline(Discipline discipline
			)
		{
			this.discipline = discipline;
		}

		public virtual void SetScore(int score)
		{
			this.score = score;
		}

		public virtual Teacher GetTeacher()
		{
			return teacher;
		}

		public virtual void SetTeacher(Teacher teacher)
		{
			this.teacher = teacher;
		}

		public override string ToString()
		{
			return "disc.=" + discipline.GetName() + " | teacher=" + teacher.GetName() + " | student="
				 + student.GetName() + " | date=" + date + " | score=" + score;
		}

		public virtual Student GetStudent()
		{
			return student;
		}

		public virtual void SetStudent(Student student)
		{
			this.student = student;
		}
	}
}
