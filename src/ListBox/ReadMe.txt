To get DevAge to run on mono, I made this change to CommonImages.cs:

		//these die under mono

			//m_SortDown = ExtractImage("SortDown.ico");
			//m_SortUp = ExtractImage("SortUp.ico");

			mRightArrow = ExtractCursor("right.cur");
			mLeftArrow = ExtractCursor("left.cur");


			//jdh
			m_SortDown = new Bitmap(8, 8);
			m_SortUp = new Bitmap(8, 8);
			//mRightArrow = new Bitmap(8, 8);
			//mLeftArrow = new Bitmap(8, 8);
