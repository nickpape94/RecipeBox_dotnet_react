import React, { Fragment, useState } from 'react';
import { setAlert } from '../../actions/alert';
import { passwordReset } from '../../actions/email';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';

const PasswordReset = ({ passwordReset }) => {
	const [ formData, setFormData ] = useState({
		email: ''
	});

	const { email } = formData;

	// console.log(email);

	const onChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

	const onSubmit = async (e) => {
		e.preventDefault();
		passwordReset(email);
	};

	return (
		<Fragment>
			<h1 className='large text-primary'>Recover your account</h1>
			<p className='lead'>Enter the email associated with your account</p>
			<form className='form' onSubmit={(e) => onSubmit(e)}>
				<div className='form-group'>
					<input
						type='email'
						placeholder='Email Address'
						name='email'
						value={email}
						onChange={(e) => onChange(e)}
						required
					/>
				</div>

				<input type='submit' className='btn btn-primary' value='Send' />
			</form>
		</Fragment>
	);
};

PasswordReset.propTypes = {
	passwordReset: PropTypes.func.isRequired
};

// const mapStateToProps = (state) => ({

// })

export default connect(null, { passwordReset })(PasswordReset);
