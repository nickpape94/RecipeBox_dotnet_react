import React, { Fragment, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Redirect, withRouter, Prompt } from 'react-router-dom';
import { connect } from 'react-redux';
import { setAlert } from '../../actions/alert';
import { getPost, updatePost } from '../../actions/post';
import Spinner from '../layout/Spinner';

const EditPost = ({ getPost, updatePost, post: { post, loading }, auth: { user }, match, history }) => {
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ isDataChanged, setDataChanged ] = useState(false);
	const [ isError, setError ] = useState(false);

	const [ formData, setFormData ] = useState({
		nameOfDish: '',
		description: '',
		ingredients: '',
		method: '',
		prepTime: '',
		cookingTime: '',
		feeds: '',
		cuisine: ''
	});

	useEffect(
		() => {
			getPost(match.params.id, setLoadingPage);

			setFormData({
				nameOfDish: loading || !post.nameOfDish ? '' : post.nameOfDish,
				description: loading || !post.description ? '' : post.description,
				ingredients: loading || !post.ingredients ? '' : post.ingredients,
				method: loading || !post.method ? '' : post.method,
				prepTime: loading || !post.prepTime ? '' : post.prepTime,
				cookingTime: loading || !post.cookingTime ? '' : post.cookingTime,
				feeds: loading || !post.feeds ? '' : post.feeds,
				cuisine: loading || !post.cuisine ? '' : post.cuisine
			});
		},
		[ loading ]
	);

	useEffect(
		() => {
			if (isError) {
				window.scrollTo(0, 0);
			}
		},
		[ isError ]
	);

	const { cuisine, nameOfDish, description, ingredients, method, prepTime, cookingTime, feeds } = formData;

	const onChange = (e) => {
		setFormData({ ...formData, [e.target.name]: e.target.value });
		setDataChanged(true);
	};

	const onSubmit = async (e) => {
		e.preventDefault();
		setDataChanged(false);
		updatePost(user.id, match.params.id, setLoadingPage, setError, history, {
			nameOfDish,
			description,
			ingredients,
			method,
			prepTime,
			cookingTime,
			feeds,
			cuisine
		});
	};

	if (loading || loadingPage) {
		return <Spinner />;
	}

	return user && user.id !== post.userId ? (
		<Redirect to={`/posts/${match.params.id}`} />
	) : (
		<Fragment>
			<Prompt when={isDataChanged} message='Are you sure you want to leave? Any changes made will be lost.' />
			<h1 className='large text-primary'>Edit your recipe</h1>
			<form className='form' onSubmit={(e) => onSubmit(e)}>
				<div className='form-group'>
					<select
						name='cuisine'
						type='text'
						placeholder='Select Cuisine'
						name='cuisine'
						value={cuisine}
						required
						onChange={(e) => onChange(e)}
					>
						<option value=''>* Select Cuisine</option>
						<option value='African'>African</option>
						<option value='American'>American</option>
						<option value='British'>British</option>
						<option value='Caribbean'>Caribbean</option>
						<option value='Chinese'>Chinese</option>
						<option value='East European'>East European</option>
						<option value='French'>French</option>
						<option value='Greek'>Greek</option>
						<option value='Indian'>Indian</option>
						<option value='Italian'>Italian</option>
						<option value='Japanese'>Japanese</option>
						<option value='Korean'>Korean</option>
						<option value='Mexican'>Mexican</option>
						<option value='North African'>Middle Eastern</option>
						<option value='Pakistani'>Pakistani</option>
						<option value='Portuguese'>Portuguese</option>
						<option value='South American'>South American</option>
						<option value='Spanish'>Spanish</option>
						<option value='Thai and South-East Asian'>Thai and South-East Asian</option>
						<option value='Turkish and Middle Eastern'>Turkish and Middle Eastern</option>
						<option value='Other'>Other</option>
					</select>
					<small className='form-text'>Please specify which cusine type this is</small>
				</div>
				<div className='form-group'>
					<input
						type='nameOfDish'
						placeholder='Name of Dish'
						name='nameOfDish'
						value={nameOfDish}
						required
						onChange={(e) => onChange(e)}
					/>
				</div>
				<div className='form-group'>
					<textarea
						cols='30'
						rows='5'
						placeholder='Description'
						type='description'
						name='description'
						value={description}
						required
						onChange={(e) => onChange(e)}
					/>
				</div>
				<div className='form-group'>
					<textarea
						cols='30'
						rows='5'
						placeholder='Ingredients'
						type='ingredients'
						name='ingredients'
						value={ingredients}
						required
						onChange={(e) => onChange(e)}
					/>
					<small className='form-text'>
						Please use comma separated values (eg.Beef, Tomatoes, Coriander, Turmeric)
					</small>
				</div>
				<div className='form-group'>
					<textarea
						cols='30'
						rows='20'
						placeholder='Method'
						type='method'
						name='method'
						value={method}
						required
						onChange={(e) => onChange(e)}
					/>
					<small className='form-text'>Please leave a line and numerate each stage of the method (eg)</small>
					<small className='form-text'>(1) Prepare marinade</small>
					<small className='form-text'>(2) Let meat marinade for 5 hours</small>
				</div>
				<span className='form-group'>
					<input
						type='preptime'
						placeholder='Prep time'
						name='prepTime'
						value={prepTime}
						required
						onChange={(e) => onChange(e)}
					/>
				</span>
				<span className='form-group'>
					<input
						type='cookingtime'
						placeholder='Cooking time'
						name='cookingTime'
						value={cookingTime}
						required
						onChange={(e) => onChange(e)}
					/>
				</span>
				<span className='form-group'>
					<input
						type='feeds'
						placeholder='Feeds'
						name='feeds'
						value={feeds}
						required
						onChange={(e) => onChange(e)}
					/>
				</span>
				<div className='form-group'>
					<input type='submit' className='btn btn-primary' value='Save and continue' />
				</div>
			</form>
		</Fragment>
	);
};

EditPost.propTypes = {
	getPost: PropTypes.func.isRequired,
	updatePost: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth,
	post: state.post
});

export default connect(mapStateToProps, { getPost, updatePost })(withRouter(EditPost));
