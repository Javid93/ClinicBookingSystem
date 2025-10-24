import { useState, useEffect } from "react";
import Layout from "@/components/Layout";
import '@/components/Layout.css'
import '@/pages/About/About.css'

export default function About()  {
  const [loading, setloading] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => setloading(false), 1000);
    return () => clearTimeout(timer);
  }, []);


  if (loading) {
    return (
      <Layout>
        <div className="loader"></div>
      </Layout>
    );
  }
  
  return (
    <Layout>
      <div className="about-page-wrapper">

     <div className="about-container">
      <h1>About NorClinic</h1>
      <p className="intro">
        NorClinic is a modern healtcare appointment platform designed to make your experience simple, fast and reliable.
        We belive in bridging the gap between patients and medical professionals using seamless technology.
      </p>

      <section className="section">
        <h2>🏥 Our Mission</h2>
        <p>
          Our mission is to ensure every patient gets timely access to trusted healtcare providers.
          Whether it's a routine checkup or specialized care, NorClinic makes it easier than ever to book and manage appointments.
        </p>
      </section>

      <section className="section">
      <h2>💡 What We Offer</h2>
      <ul>
        <li>🗓️ Easy and intuitive appointment booking</li>
        <li>🔎 Search for doctors by name, clinic, or specialty</li>
        <li>📅 Personalized dashboard to manage appointments</li>
        <li>🔐 Secure data and patient privacy compliance</li>
      </ul>
      </section>

      <section className="section">
        <h2> 🌐 Why Choose Us</h2>
        <p>
          NorClinic blends modern UX with robust backend systems, delivering both functionality and ease of use.
          Trusted by thousands, we are committed to continuously improving your healtcare experience.
        </p>
      </section>
    </div>
  </div>
  </Layout>
  );
};


