// Contact.tsx
import React, { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import './Contact.css';

const Contact: React.FC = () => {
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const timeout = setTimeout(() => {
      setIsLoading(false);
    }, 500); // delay to show loader

    return () => clearTimeout(timeout);
  }, []);

  return (
    <Layout>
      {isLoading ? (
        <div className="loader" />
      ) : (
        <div className="contact-wrapper">
  <div className="contact-box">
    <h2>Contact Us</h2>
    <p className="contact-info">📍 NorClinic, Oslo, Norway</p>
    <p className="contact-info">📧 Email: support@norclinic.no</p>
    <p className="contact-info">📞 Phone: +47 123 456 78</p>
  </div>
</div>

      )}
    </Layout>
  );
};

export default Contact;
