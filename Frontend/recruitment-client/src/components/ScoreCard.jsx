function AIScoreCard({ score }) {

  return (
    <div
      className="
      bg-gradient-to-br
      from-teal-900
      to-teal-600
      rounded-3xl
      p-8
      text-white
      shadow-lg
      flex
      flex-col
      items-center
      "
    >

      <h2 className="text-lg font-semibold">
        AI Resume Score
      </h2>


      <div
        className="
        w-36
        h-36
        rounded-full
        border-8
        border-teal-200
        flex
        items-center
        justify-center
        mt-6
        "
      >

        <span className="text-4xl font-bold">
          {score}%
        </span>

      </div>


      <p className="text-teal-100 mt-5 text-center">
        Your resume strength based on AI analysis
      </p>


    </div>
  );
}


export default AIScoreCard;