import React, { Fragment, useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Link, Redirect } from 'react-router-dom';
import { setAlert } from '../../actions/alert';
import { register } from '../../actions/auth';
import Spinner from '../layout/Spinner';
import PropTypes from 'prop-types';

const Register = ({ setAlert, register, isAuthenticated, history }) => {
	const [ formData, setFormData ] = useState({
		username: '',
		email: '',
		password: '',
		password2: ''
	});

	const { username, email, password, password2 } = formData;

	const [ submitted, submitting ] = useState(false);

	// useEffect(
	// 	() => {
	// 		if (isAuthenticated) {
	// 			history.push('/posts');
	// 		}
	// 	},
	// 	[ isAuthenticated ]
	// );

	if (isAuthenticated) {
		return <Redirect to='/posts' />;
	}

	if (submitted) {
		return <Spinner />;
	}

	const onChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

	const onSubmit = async (e) => {
		submitting(true);
		e.preventDefault();
		if (password !== password2) {
			submitting(false);
			setAlert('Passwords do not match', 'danger');
		}
		if (username.length < 4 || username.length > 20) {
			submitting(false);
			setAlert('User name must be between 4 and 20 characters long', 'danger');
		} else {
			register({ username, email, password, submitting });
		}
	};

	// console.log(submitted);

	return (
		<Fragment>
			<h1 className='large text-primary'>Sign Up</h1>
			<p className='lead'>
				<i className='fas fa-user' /> Create Your Account
			</p>
			<form className='form' onSubmit={(e) => onSubmit(e)}>
				<div className='form-group'>
					<input
						type='text'
						placeholder='Username'
						name='username'
						value={username}
						onChange={(e) => onChange(e)}
					/>
				</div>
				<div className='form-group'>
					<input
						type='email'
						placeholder='Email Address'
						name='email'
						value={email}
						onChange={(e) => onChange(e)}
					/>
				</div>
				<div className='form-group'>
					<input
						type='password'
						placeholder='Password'
						name='password'
						value={password}
						onChange={(e) => onChange(e)}
					/>
				</div>
				<div className='form-group'>
					<input
						type='password'
						placeholder='Confirm Password'
						name='password2'
						value={password2}
						onChange={(e) => onChange(e)}
					/>
				</div>
				{username.length === 0 || email.length === 0 || password.length === 0 || password2.length === 0 ? (
					<input type='submit' className='btn btn-primary' value='Register' disabled />
				) : (
					// console.log('btn disabled')
					<input type='submit' className='btn btn-primary' value='Register' />
					// console.log('btn enabled')
				)}
			</form>
			<p className='my-1'>
				Already have an account? <Link to='/login'>Sign In</Link>
			</p>
		</Fragment>
	);
};

Register.propTypes = {
	setAlert: PropTypes.func.isRequired,
	register: PropTypes.func.isRequired,
	isAuthenticated: PropTypes.bool
};

const mapStateToProps = (state) => ({
	isAuthenticated: state.auth.isAuthenticated
});

export default connect(mapStateToProps, { setAlert, register })(Register);
