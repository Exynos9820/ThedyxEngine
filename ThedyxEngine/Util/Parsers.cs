namespace ThedyxEngine.Util {
    /**
     * \class Parsers
     * \brief Provides custom parsers
     */
    public static class Parsers {
        /** 
         * Parses a string representation of a point to a Point object.
         * \param point The string representation of the point.
         * \return A Point object parsed from the string.
         */
        public static Point ParsePoint(string point) {
            var parts = point.Split(',');
            return new Point(double.Parse(parts[0]), double.Parse(parts[1]));
        }
    }
}
