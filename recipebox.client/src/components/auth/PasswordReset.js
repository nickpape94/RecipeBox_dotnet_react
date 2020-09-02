import React, { Fragment, useState } from 'react';
import { connect } from 'react-redux';
import { Link, Redirect } from 'react-router-dom';
import { setAlert } from '../../actions/alert';
import { resetPassword } from '../../actions/auth';
import PropTypes from 'prop-types';

const PasswordReset = ({ setAlert, resetPassword, isAuthenticated }) => {
	const [ formData, setFormData ] = useState({
		newPassword: '',
		confirmPassword: ''
	});

	const { newPassword, confirmPassword } = formData;

	const onChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

	const queryString = window.location.search;
	const urlParams = new URLSearchParams(queryString);
	const token = urlParams.get('token');
	const email = urlParams.get('email');
	// console.log(email);

	const onSubmit = async (e) => {
		e.preventDefault();
		if (newPassword !== confirmPassword) {
			setAlert('Passwords do not match', 'danger');
		} else {
			resetPassword({ token, email, newPassword, confirmPassword });
			setAlert('Password has been updated.', 'success');
		}
	};

	if (isAuthenticated) {
		return <Redirect to='/posts' />;
	}

	return (
		<Fragment>
			<h1 className='large text-primary'>Reset your password</h1>
			<form className='form' onSubmit={(e) => onSubmit(e)}>
				<div className='form-group'>
					<input
						type='password'
						placeholder='Password'
						name='newPassword'
						value={newPassword}
						onChange={(e) => onChange(e)}
					/>
				</div>
				<div className='form-group'>
					<input
						type='password'
						placeholder='Confirm Password'
						name='confirmPassword'
						value={confirmPassword}
						onChange={(e) => onChange(e)}
					/>
				</div>
				<input type='submit' className='btn btn-primary' value='Register' />
			</form>
		</Fragment>
	);
};

PasswordReset.propTypes = {
	setAlert: PropTypes.func.isRequired,
	resetPassword: PropTypes.func.isRequired,
	isAuthenticated: PropTypes.bool
};

const mapStateToProps = (state) => ({
	isAuthenticated: state.auth.isAuthenticated
});

export default connect(mapStateToProps, { setAlert, resetPassword })(PasswordReset);
