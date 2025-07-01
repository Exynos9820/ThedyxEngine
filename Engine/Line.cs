namespace ThedyxEngine.Engine {
    /**
     * \brief Represents a line segment in 2D space.
     * Used for calculating the overlap between two line segments.
     */
    public struct Line {
        public Point Start, End;  // Start and end points of the line segment

        /**
         * \brief Constructor for the Line class.
         * \param start The start point of the line segment.
         * \param end The end point of the line segment.
         */
        public Line(Point start, Point end) {
            Start = start;
            End = end;
        }

        /**
         * \brief Calculate the length of the line segment.
         * \return The length of the line segment.
         */
        public double Length() {
            return Math.Sqrt((End.X - Start.X) * (End.X - Start.X) + (End.Y - Start.Y) * (End.Y - Start.Y));
        }

        /**
         * \brief Determine if two line segments are collinear.
         * \param l1 The first line segment.
         * \param l2 The second line segment.
         * \return True if the two line segments are collinear, false otherwise.
         */
        public static bool AreCollinear(Line l1, Line l2) {
            // Check if the two lines are collinear by cross product being zero
            return (l1.End.X - l1.Start.X) * (l2.End.Y - l2.Start.Y) ==
                   (l2.End.X - l2.Start.X) * (l1.End.Y - l1.Start.Y);
        }

        /**
         * \brief Calculate the overlap length between two collinear line segments.
         * \return The overlap length between the two line segments.
         */
        public static double CalculateOverlap(Line l1, Line l2) {
            if (!AreCollinear(l1, l2))
                return 0;

            // Sort points to make comparison easier
            Point l1P1 = l1.Start.X < l1.End.X ? l1.Start : l1.End;
            Point l1P2 = l1.Start.X < l1.End.X ? l1.End : l1.Start;
            Point l2P1 = l2.Start.X < l2.End.X ? l2.Start : l2.End;
            Point l2P2 = l2.Start.X < l2.End.X ? l2.End : l2.Start;

            // Check for overlap
            if (l1P2.X < l2P1.X || l2P2.X < l1P1.X)
                return 0;  // No overlap

            // Calculate overlapxxscd   4           QWERTYU'H
            double start = Math.Max(l1P1.X, l2P1.X);
            double end = Math.Min(l1P2.X, l2P2.X);
            return end - start;
        }
    }

}
