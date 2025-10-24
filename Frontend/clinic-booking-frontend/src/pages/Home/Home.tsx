import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import Layout from "@/components/Layout";
import "./Home.css";

export default function Home() {
  const [isLoading, setloading] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
      setloading(false);
    }, 300);

    return() => clearTimeout(timer);
  }, []);

  if (isLoading) {
    return (
      <Layout>
        <div className="loader"></div>
      </Layout>
    );
  }


return (
  <Layout>
    <div className="home-content">
      <h1>Welcome to Health Care Medical Clinic</h1>
      <p>Your trusted partner in health care appointments</p>
      <Link to="/book" className="cta-button">
        Book Appointment
      </Link>
    </div>
  </Layout>
);

}
