import { useParams } from "react-router-dom";

function InterviewDetails() {
  const { id } = useParams();

  return (
    <div>
      <h1>Interview Details</h1>
      <p>Interview ID: {id}</p>
    </div>
  );
}

export default InterviewDetails;
